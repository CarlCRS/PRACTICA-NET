-- 05 - SP transaccional: registra pedido, detalle, pago y descuenta inventario
USE LogiTrackDB;
GO

-- Tipo de tabla para pasar el detalle del pedido al SP.
CREATE TYPE dbo.PedidoDetalleTipo AS TABLE (
    ProductoId INT NOT NULL,
    Cantidad   INT NOT NULL
);
GO

CREATE PROCEDURE dbo.sp_RegistrarPedido
    @ClienteId  INT,
    @MetodoPago VARCHAR(20),
    @Detalles   dbo.PedidoDetalleTipo READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Serializa el registro de pedidos: un pedido a la vez, así dos no descuentan el mismo stock.
        DECLARE @lock INT;
        EXEC @lock = sp_getapplock
            @Resource  = 'LogiTrack:RegistrarPedido',
            @LockMode  = 'Exclusive',
            @LockOwner = 'Transaction',
            @LockTimeout = 5000;

        IF @lock < 0
            THROW 50006, 'No se pudo obtener el bloqueo de inventario (otro pedido en curso).', 1;

        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Id = @ClienteId)
            THROW 50001, 'El cliente no existe.', 1;

        IF NOT EXISTS (SELECT 1 FROM @Detalles)
            THROW 50002, 'El pedido debe tener al menos un producto.', 1;

        -- 1) Cabecera del pedido
        INSERT INTO Pedidos (ClienteId, Fecha, Estado, Total)
        VALUES (@ClienteId, GETDATE(), 'Pendiente', 0);

        DECLARE @PedidoId INT = SCOPE_IDENTITY();

        -- 2) Detalle con el precio actual del producto
        INSERT INTO PedidoDetalles (PedidoId, ProductoId, Cantidad, PrecioUnitario)
        SELECT @PedidoId, d.ProductoId, d.Cantidad, p.PrecioBase
        FROM @Detalles d
        JOIN Productos p ON p.Id = d.ProductoId;

        IF (SELECT COUNT(*) FROM PedidoDetalles WHERE PedidoId = @PedidoId)
           <> (SELECT COUNT(*) FROM @Detalles)
            THROW 50003, 'Uno o más productos del detalle no existen.', 1;

        -- 3) Un solo detalle por producto
        IF (SELECT COUNT(*) FROM @Detalles) <> (SELECT COUNT(DISTINCT ProductoId) FROM @Detalles)
            THROW 50005, 'El detalle no puede repetir un producto.', 1;

        -- 4) Descontar inventario: la fila solo se actualiza si hay stock suficiente.
        --    La fila de INVENTARIO con stock insufficiente no se actualiza y el conteo falla.
        DECLARE @FilasConInventario INT =
            (SELECT COUNT(*) FROM @Detalles d JOIN Inventario i ON i.ProductoId = d.ProductoId);

        UPDATE i
        SET i.StockActual = i.StockActual - d.Cantidad
        FROM Inventario i
        JOIN @Detalles d ON d.ProductoId = i.ProductoId
        WHERE i.StockActual >= d.Cantidad;

        IF @@ROWCOUNT <> @FilasConInventario
            THROW 50004, 'Stock insuficiente para uno o más productos.', 1;

        -- 5) Total del pedido = suma del detalle
        UPDATE Pedidos
        SET Total = (SELECT SUM(Cantidad * PrecioUnitario) FROM PedidoDetalles WHERE PedidoId = @PedidoId)
        WHERE Id = @PedidoId;

        -- 6) Pago aprobado y estado confirmado
        INSERT INTO Pagos (PedidoId, Metodo, Monto, Comprobante, Fecha, Estado)
        SELECT @PedidoId,
               @MetodoPago,
               Total,
               CONCAT('CMP-', @PedidoId),
               GETDATE(),
               'Aprobado'
        FROM Pedidos
        WHERE Id = @PedidoId;

        UPDATE Pedidos SET Estado = 'Confirmado' WHERE Id = @PedidoId;

        COMMIT TRANSACTION;

        SELECT @PedidoId AS PedidoId;
    END TRY
    BEGIN CATCH
        -- Ante cualquier error se deshace todo: no queda pedido a medias ni stock descontado.
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO
