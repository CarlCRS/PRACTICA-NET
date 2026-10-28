namespace Practica_net1_LogiTrack.Repositorio;

public class Producto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioBase { get; set; }
    public string Tipo { get; set; } = string.Empty;
}
