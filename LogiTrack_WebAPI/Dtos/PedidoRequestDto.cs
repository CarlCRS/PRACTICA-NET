namespace LogiTrack_WebAPI.Dtos;

public class PedidoRequestDto
{
    public int ClienteId { get; set; }
    public string MetodoPago { get; set; } = "Tarjeta";
    public List<DetallePedidoDto> Detalles { get; set; } = new();
}
