namespace LogiTrack_WebAPI.Dtos;

public class PedidoDetalleDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
