namespace Practica_net1_LogiTrack.Repositorio;

public class Pedido
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<PedidoDetalle> Detalles { get; set; } = new();
}
