-- 03 - Índices no clustered para las consultas frecuentes de los reportes
USE LogiTrackDB;
GO

-- Historial por cliente y ventas por cliente (columna + fecha).
CREATE INDEX IX_Pedidos_ClienteId_Fecha ON Pedidos(ClienteId, Fecha);

-- Reportes por periodo.
CREATE INDEX IX_Pedidos_Fecha ON Pedidos(Fecha);

-- Ranking de productos vendidos e ingresos por tipo.
CREATE INDEX IX_PedidoDetalles_ProductoId ON PedidoDetalles(ProductoId);

-- Búsqueda del detalle de un pedido.
CREATE INDEX IX_PedidoDetalles_PedidoId ON PedidoDetalles(PedidoId);

-- Agrupación por tipo de producto.
CREATE INDEX IX_Productos_Tipo ON Productos(Tipo);
GO
