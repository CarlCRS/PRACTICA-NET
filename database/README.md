# LogiTrack - Base de Datos (Módulo 03)

Diseño de la base `LogiTrackDB` para SQL Server, mapeado desde el dominio C# de los módulos 01 y 02.

## Cómo reconstruir la base desde cero

Instancia usada: **LocalDB** (el login de Windows es `sysadmin` ahí; en `.\SQLEXPRESS` este usuario solo tiene `public` y no puede crear bases).

```bat
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\01_crear_base.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\02_tablas.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\03_indices.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\04_datos_semilla.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\05_sp_registrar_pedido.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\06_trigger_auditoria.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\07_funciones.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -i database\scripts\08_reportes.sql
```

El script `01` borra la base si existe y la crea limpia, así que se puede reconstruir siempre desde cero. `08` solo consulta y muestra los reportes.

## Requisito de la actividad → dónde se cumple

| Requisito del módulo | Dónde |
|---|---|
| Persistir usuarios y roles | `02_tablas.sql` → `Roles`, `Usuarios` (FK) |
| Persistir los 3 tipos de producto sin perder diferencias | `02_tablas.sql` → `Productos` + `ProductoFisicos` / `ProductoDigitales` / `Servicios` |
| Pedidos y detalle | `Pedidos`, `PedidoDetalles` (precio histórico) |
| Pagos asociados a pedidos | `Pagos` (FK a `Pedidos`) |
| Inventario para los que lo requieran | `Inventario` (solo físicos) |
| Auditoría de cambios | `06_trigger_auditoria.sql` → `trg_Inventario_Auditoria` |
| Modelo normalizado, PK/FK | `02_tablas.sql` |
| Índices no clustered | `03_indices.sql` (tabla más abajo) |
| SP transaccional de pedido + descuento de stock | `05_sp_registrar_pedido.sql` |
| Concurrencia sin vender stock inexistente | `sp_getapplock` + `UPDATE ... WHERE StockActual >= Cantidad` + `CHECK (StockActual >= 0)` |
| Trigger de auditoría | `06_trigger_auditoria.sql` |
| CTE recursivo | `08_reportes.sql` → reporte 6 (árbol de categorías) |
| Función de ventana | `08_reportes.sql` → reporte 7 (`RANK() OVER (PARTITION BY periodo ...)`) |
| Función de tabla | `07_funciones.sql` → `fn_HistorialCliente` |
| Scripts versionados y ejecutables | `database/scripts/01..08` |
| Reconstrucción documentada | este README |

## Modelo relacional

```
Roles ──< Usuarios ──< Pedidos >── Pagos
              │            │
              │      PedidoDetalles >── Productos >── Categorias (auto-referenciada)
              │                              │
              │                        ProductoFisicos (1-1)
              │                        ProductoDigitales (1-1)
              │                        Servicios (1-1)
              │                              │
              │                        Inventario (solo físicos)
              │                              │
              └──────────────── AuditoriaInventario (trigger)
```

### Tablas

- **Roles**: `Id`, `Nombre` (Cliente / Bodeguero / Administrador).
- **Usuarios**: `Id`, `Nombre`, `Email` único, `RolId` FK.
- **Categorias**: `Id`, `Nombre`, `CategoriaPadreId` FK a sí misma (sostiene el CTE).
- **Productos**: `Id`, `SKU` único, `Nombre`, `PrecioBase`, `Tipo` (CHECK), `CategoriaId`, `Activo`.
- **ProductoFisicos**: `ProductoId` PK/FK, `PesoKg`.
- **ProductoDigitales**: `ProductoId` PK/FK, `UrlDescarga`.
- **Servicios**: `ProductoId` PK/FK, `FechaAgenda`, `DuracionHoras`.
- **Inventario**: `ProductoId` PK/FK, `StockInicial`, `StockActual` con `CHECK (StockActual >= 0)`.
- **Pedidos**: `Id`, `ClienteId` FK, `Fecha`, `Estado` (CHECK de 6 estados), `Total`.
- **PedidoDetalles**: `Id`, `PedidoId`, `ProductoId`, `Cantidad`, `PrecioUnitario` (histórico).
- **Pagos**: `Id`, `PedidoId`, `Metodo`, `Monto`, `Comprobante` único, `Estado`.
- **AuditoriaInventario**: `ProductoId`, `Operacion` (Venta/Reposicion/Ajuste), `StockAnterior`, `StockNuevo`.

**Tipos de producto**: tabla base + tablas hijas (TPT), espejo de la herencia C#. Lo común en `Productos`, lo propio en cada hija, y la columna `Tipo` para agrupar en reportes sin comparar tablas.

## Índices (03_indices.sql)

| Índice | Consulta que optimiza |
|---|---|
| `IX_Pedidos_ClienteId_Fecha` | Historial de cliente, ventas por cliente, riesgo |
| `IX_Pedidos_Fecha` | Reportes por periodo |
| `IX_PedidoDetalles_ProductoId` | Ranking de productos, ingresos por tipo |
| `IX_PedidoDetalles_PedidoId` | Detalle de un pedido |
| `IX_Productos_Tipo` | Agrupación por tipo |
| `UQ_Usuarios_Email`, `UQ_Productos_SKU`, `UQ_Pagos_Comprobante`, `UQ_Roles_Nombre` | unicidad |

## SP `sp_RegistrarPedido`

Parámetros: `@ClienteId`, `@MetodoPago` y `@Detalles` (tipo de tabla `PedidoDetalleTipo` con producto y cantidad).

Pasos: valida cliente y detalle → inserta `Pedidos` y `PedidoDetalles` con el precio actual → descuenta inventario → calcula `Total` → registra pago aprobado → deja el pedido `Confirmado`.

- **Errores**: `SET XACT_ABORT ON`, `TRY/CATCH`, `ROLLBACK` en el `CATCH` y `THROW`. Si algo falla no queda pedido a medias ni stock descontado.
- **Concurrencia**: al iniciar la transacción se toma un lock de aplicación (`sp_getapplock`) que serializa el registro de pedidos. Además el descuento es un `UPDATE` condicionado a `StockActual >= Cantidad`, así una fila sin stock no se actualiza y el SP lanza error. El `CHECK (StockActual >= 0)` es la última defensa.
- **Aislamiento**: `READ COMMITTED` (el default). El lock de aplicación da la exclusión necesaria para el stock; no hace falta `SERIALIZABLE` global.

Ejemplo:

```sql
USE LogiTrackDB;
DECLARE @d dbo.PedidoDetalleTipo;
INSERT INTO @d VALUES (1, 2), (5, 1);   -- 2 notebooks + 1 licencia
EXEC dbo.sp_RegistrarPedido @ClienteId = 1, @MetodoPago = 'Tarjeta', @Detalles = @d;
```

Resultado esperado: se crea el pedido, el stock del notebook baja, el trigger deja una fila en `AuditoriaInventario` y se registra el pago.

## Pruebas realizadas

| Prueba | Resultado |
|---|---|
| Pedido válido (2 notebooks + 1 licencia) | Pedido creado, stock 8→6, auditoría `Venta 8→6`, pago `Aprobado` |
| Pedido con 999 teclados (stock 1) | `Stock insuficiente para uno o más productos.`, sin pedido y sin cambios de stock |
| Dos pedidos simultáneos por la última unidad del teclado | Uno confirma, el otro recibe `Stock insuficiente`; stock final 0, nunca negativo |

## Datos semilla

3 roles, 7 usuarios, 7 categorías, 9 productos (4 físicos con stock 10/6/6/25, 3 digitales, 2 servicios), 17 pedidos con estados variados y fechas fijas de 2026, 26 líneas de detalle y 13 pagos. Fechas fijas para que los reportes den siempre el mismo resultado.

## Qué queda en SQL y qué en la app

| En la base | En la app (módulos 01/02/04) |
|---|---|
| Registro transaccional del pedido y descuento de stock | Cálculo del costo de envío por peso |
| Auditoría de inventario (trigger) | Permisos por rol |
| Reportes de lectura, CTE, ventanas, TVF | Validaciones de entrada |
| Pagos y su comprobante | Elección del medio de pago |
