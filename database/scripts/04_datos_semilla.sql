-- 04 - Datos semilla (mismos datos del dominio C# del módulo 02)
USE LogiTrackDB;
GO

SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (Id, Nombre) VALUES
    (1, 'Cliente'), (2, 'Bodeguero'), (3, 'Administrador');
SET IDENTITY_INSERT Roles OFF;

SET IDENTITY_INSERT Usuarios ON;
INSERT INTO Usuarios (Id, Nombre, Email, RolId, FechaAlta) VALUES
    (1, 'María Gómez',      'maria@mail.com',            1, '2026-01-10'),
    (2, 'Carlos Ruiz',      'carlos@mail.com',           1, '2026-01-12'),
    (3, 'Lucía Fernández',  'lucia@mail.com',            1, '2026-02-01'),
    (4, 'Diego Torres',     'diego@mail.com',            1, '2026-02-15'),
    (5, 'Sofía Méndez',     'sofia@mail.com',            1, '2026-03-01'),
    (6, 'Juan Pérez',       'juan.bodega@logitrack.com', 2, '2026-01-05'),
    (7, 'Ana Sosa',         'ana.admin@logitrack.com',   3, '2026-01-05');
SET IDENTITY_INSERT Usuarios OFF;

-- Árbol de categorías (para el CTE recursivo del script 08).
SET IDENTITY_INSERT Categorias ON;
INSERT INTO Categorias (Id, Nombre, CategoriaPadreId) VALUES
    (1, 'Bodega',        NULL),
    (2, 'Computación',   1),
    (3, 'Notebooks',     2),
    (4, 'Monitores',     2),
    (5, 'Periféricos',   2),
    (6, 'Digitales',     NULL),
    (7, 'Servicios',     NULL);
SET IDENTITY_INSERT Categorias OFF;

-- Catálogo: 4 físicos, 3 digitales, 2 servicios.
SET IDENTITY_INSERT Productos ON;
INSERT INTO Productos (Id, SKU, Nombre, PrecioBase, Tipo, CategoriaId) VALUES
    (1, 'F-NOTE-01',  'Notebook Lenovo',           850000, 'Fisico',   3),
    (2, 'F-MONI-02',  'Monitor 24 pulgadas',       180000, 'Fisico',   4),
    (3, 'F-TECL-03',  'Teclado Mecánico',           45000, 'Fisico',   5),
    (4, 'F-IMPR-04',  'Impresora Térmica',         120000, 'Fisico',   5),
    (5, 'D-AV-001',   'Licencia Antivirus',         12000, 'Digital',  6),
    (6, 'D-EBOOK-02', 'Ebook C#',                    3000, 'Digital',  6),
    (7, 'D-CURSO-03', 'Curso SQL Online',           25000, 'Digital',  6),
    (8, 'S-INST-01',  'Instalación y configuración',45000, 'Servicio', 7),
    (9, 'S-MANT-02',  'Mantenimiento anual',       150000, 'Servicio', 7);
SET IDENTITY_INSERT Productos OFF;

INSERT INTO ProductoFisicos (ProductoId, PesoKg) VALUES
    (1, 3.2), (2, 5.5), (3, 1.2), (4, 2.0);

INSERT INTO ProductoDigitales (ProductoId, UrlDescarga) VALUES
    (5, 'https://descargas.logitrack.com/av'),
    (6, 'https://descargas.logitrack.com/ebook-cs'),
    (7, 'https://descargas.logitrack.com/curso-sql');

INSERT INTO Servicios (ProductoId, FechaAgenda, DuracionHoras) VALUES
    (8, '2026-11-15T10:00:00', 2.0),
    (9, '2026-11-30T09:00:00', 8.0);

-- Stock inicial y stock actual coherente con las ventas semilla.
INSERT INTO Inventario (ProductoId, StockInicial, StockActual) VALUES
    (1, 10, 8),
    (2, 6, 2),
    (3, 6, 1),
    (4, 25, 22);

-- Pedidos con fechas fijas y estados variados.
SET IDENTITY_INSERT Pedidos ON;
INSERT INTO Pedidos (Id, ClienteId, Fecha, Estado, Total) VALUES
    (1,  1, '2026-05-20T10:00:00', 'Entregado',     862000),
    (2,  2, '2026-05-30T11:30:00', 'Entregado',     225000),
    (3,  1, '2026-06-29T09:15:00', 'Entregado',      28000),
    (4,  3, '2026-07-09T15:00:00', 'Enviado',       150000),
    (5,  2, '2026-07-24T12:45:00', 'Enviado',       850000),
    (6,  4, '2026-08-08T16:20:00', 'Entregado',     132000),
    (7,  1, '2026-08-28T10:10:00', 'Entregado',      45000),
    (8,  3, '2026-09-07T14:00:00', 'Confirmado',    183000),
    (9,  5, '2026-08-13T09:40:00', 'Entregado',      57000),
    (10, 5, '2026-08-16T17:05:00', 'Pendiente',       3000),
    (11, 4, '2026-09-17T11:00:00', 'Cancelado',      25000),
    (12, 2, '2026-09-27T13:30:00', 'Cancelado',     895000),
    (13, 1, '2026-10-03T10:25:00', 'Entregado',     180000),
    (14, 2, '2026-10-08T16:15:00', 'Confirmado',    205000),
    (15, 4, '2026-10-13T12:00:00', 'Confirmado',     45000),
    (16, 2, '2026-10-20T09:50:00', 'Pendiente',     165000),
    (17, 3, '2026-10-23T15:35:00', 'EnPreparacion', 120000);
SET IDENTITY_INSERT Pedidos OFF;

-- Detalle: el precio se guarda histórico (el actual del catálogo al momento del pedido).
INSERT INTO PedidoDetalles (PedidoId, ProductoId, Cantidad, PrecioUnitario)
SELECT v.PedidoId, v.ProductoId, 1, p.PrecioBase
FROM (VALUES
    (1, 1), (1, 5),
    (2, 2), (2, 3),
    (3, 7), (3, 6),
    (4, 9),
    (5, 1),
    (6, 4), (6, 5),
    (7, 3),
    (8, 2), (8, 6),
    (9, 3), (9, 5),
    (10, 6),
    (11, 7),
    (12, 1), (12, 3),
    (13, 2),
    (14, 2), (14, 7),
    (15, 3),
    (16, 3), (16, 4),
    (17, 4)
) AS v(PedidoId, ProductoId)
JOIN Productos p ON p.Id = v.ProductoId;

-- Pagos aprobados de los pedidos que llegaron a estar confirmados o más adelante.
INSERT INTO Pagos (PedidoId, Metodo, Monto, Comprobante, Fecha, Estado)
SELECT p.Id,
       CASE WHEN p.Id IN (4, 14) THEN 'Transferencia' ELSE 'Tarjeta' END,
       p.Total,
       CONCAT('CMP-', p.Id),
       p.Fecha,
       'Aprobado'
FROM Pedidos p
WHERE p.Estado IN ('Confirmado','EnPreparacion','Enviado','Entregado');
GO
