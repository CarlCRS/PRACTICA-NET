-- 07 - Función de tabla: historial de pedidos de un cliente
USE LogiTrackDB;
GO

CREATE FUNCTION dbo.fn_HistorialCliente (@ClienteId INT)
RETURNS TABLE
AS
RETURN
    SELECT
        p.Id AS PedidoId,
        p.Fecha,
        p.Estado,
        p.Total
    FROM Pedidos p
    WHERE p.ClienteId = @ClienteId;
GO
