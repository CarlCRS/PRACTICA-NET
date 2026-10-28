-- 06 - Trigger de auditoría sobre cambios de inventario
USE LogiTrackDB;
GO

CREATE TRIGGER trg_Inventario_Auditoria
ON Inventario
AFTER UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO AuditoriaInventario (ProductoId, Operacion, StockAnterior, StockNuevo)
    SELECT
        d.ProductoId,
        CASE
            WHEN i.ProductoId IS NULL THEN 'Ajuste'   -- DELETE
            WHEN i.StockActual > d.StockActual THEN 'Reposicion'
            ELSE 'Venta'
        END,
        d.StockActual,
        ISNULL(i.StockActual, 0)
    FROM deleted d
    LEFT JOIN inserted i ON i.ProductoId = d.ProductoId;
END
GO
