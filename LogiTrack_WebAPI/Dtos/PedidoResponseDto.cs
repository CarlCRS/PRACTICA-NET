namespace LogiTrack_WebAPI.Dtos;

public class PedidoResponseDto
{
    public int PedidoId { get; set; }
    public int ClienteId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<PedidoDetalleDto> Detalles { get; set; } = new();
}
