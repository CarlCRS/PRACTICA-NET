-- 08 - Reportes (equivalentes a los del módulo 02, ahora en T-SQL)
USE LogiTrackDB;
GO

PRINT '=== 1) Clientes con mayor monto comprado ===';
SELECT u.Nombre AS Cliente,
       COUNT(p.Id) AS Pedidos,
       SUM(p.Total) AS TotalComprado
FROM Pedidos p
JOIN Usuarios u ON u.Id = p.ClienteId
WHERE p.Estado <> 'Cancelado'
GROUP BY u.Id, u.Nombre
ORDER BY TotalComprado DESC;

PRINT '=== 2) Ingresos por tipo de producto y periodo ===';
SELECT CONCAT(YEAR(p.Fecha), '-', RIGHT('0' + CAST(MONTH(p.Fecha) AS VARCHAR(2)), 2)) AS Periodo,
       pr.Tipo AS TipoProducto,
       SUM(d.Cantidad) AS Unidades,
       SUM(d.Cantidad * d.PrecioUnitario) AS Ingresos
FROM Pedidos p
JOIN PedidoDetalles d ON d.PedidoId = p.Id
JOIN Productos pr ON pr.Id = d.ProductoId
WHERE p.Estado <> 'Cancelado'
GROUP BY YEAR(p.Fecha), MONTH(p.Fecha), pr.Tipo
ORDER BY Periodo, TipoProducto;

PRINT '=== 3) Productos con inventario bajo (< 50% del stock inicial) ===';
SELECT p.SKU,
       p.Nombre,
       i.StockInicial,
       i.StockActual,
       CAST(100.0 * i.StockActual / NULLIF(i.StockInicial, 0) AS DECIMAL(5,1)) AS PorcentajeRestante
FROM Inventario i
JOIN Productos p ON p.Id = i.ProductoId
WHERE i.StockActual < i.StockInicial * 0.5
ORDER BY PorcentajeRestante;

PRINT '=== 4) Clientes con pedidos en más de un estado ===';
SELECT u.Nombre AS Cliente,
       COUNT(DISTINCT p.Estado) AS EstadosDistintos,
       STRING_AGG(p.Estado, ', ') AS Estados
FROM Pedidos p
JOIN Usuarios u ON u.Id = p.ClienteId
GROUP BY u.Id, u.Nombre
HAVING COUNT(DISTINCT p.Estado) > 1
ORDER BY EstadosDistintos DESC;

PRINT '=== 5) Clientes en riesgo (sin comprar > 60 días o promedio < 30000) ===';
SELECT u.Nombre AS Cliente,
       MAX(p.Fecha) AS UltimaCompra,
       DATEDIFF(DAY, MAX(p.Fecha), GETDATE()) AS DiasSinComprar,
       CAST(AVG(p.Total) AS DECIMAL(12,2)) AS PromedioCompra
FROM Pedidos p
JOIN Usuarios u ON u.Id = p.ClienteId
WHERE p.Estado <> 'Cancelado'
GROUP BY u.Id, u.Nombre
HAVING DATEDIFF(DAY, MAX(p.Fecha), GETDATE()) > 60 OR AVG(p.Total) < 30000
ORDER BY DiasSinComprar DESC;

PRINT '=== 6) Árbol de categorías con CTE recursivo ===';
WITH ArbolCategorias AS
(
    -- Ancla: categorías raíz
    SELECT Id, Nombre, CategoriaPadreId, 0 AS Nivel,
           CAST(Nombre AS VARCHAR(500)) AS Ruta
    FROM Categorias
    WHERE CategoriaPadreId IS NULL

    UNION ALL

    -- Recursión: hijos de cada categoría
    SELECT c.Id, c.Nombre, c.CategoriaPadreId, a.Nivel + 1,
           CAST(a.Ruta + ' > ' + c.Nombre AS VARCHAR(500))
    FROM Categorias c
    JOIN ArbolCategorias a ON c.CategoriaPadreId = a.Id
)
SELECT Id, Nombre, Nivel, Ruta
FROM ArbolCategorias
ORDER BY Ruta;

PRINT '=== 7) Ranking de productos vendidos por periodo (función de ventana) ===';
WITH VentasPorPeriodo AS
(
    SELECT CONCAT(YEAR(p.Fecha), '-', RIGHT('0' + CAST(MONTH(p.Fecha) AS VARCHAR(2)), 2)) AS Periodo,
           pr.Nombre AS Producto,
           SUM(d.Cantidad * d.PrecioUnitario) AS Monto
    FROM Pedidos p
    JOIN PedidoDetalles d ON d.PedidoId = p.Id
    JOIN Productos pr ON pr.Id = d.ProductoId
    WHERE p.Estado <> 'Cancelado'
    GROUP BY YEAR(p.Fecha), MONTH(p.Fecha), pr.Nombre
)
SELECT Periodo,
       Producto,
       Monto,
       RANK() OVER (PARTITION BY Periodo ORDER BY Monto DESC) AS Ranking
FROM VentasPorPeriodo
ORDER BY Periodo, Ranking;

PRINT '=== 8) Historial de un cliente con la función de tabla ===';
SELECT * FROM dbo.fn_HistorialCliente(2) ORDER BY Fecha;

PRINT '=== 9) Auditoría de inventario (se llena con el trigger) ===';
SELECT a.Id, p.Nombre AS Producto, a.Operacion, a.StockAnterior, a.StockNuevo, a.Fecha
FROM AuditoriaInventario a
JOIN Productos p ON p.Id = a.ProductoId
ORDER BY a.Id;
GO
