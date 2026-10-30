namespace LogiTrack_WebAPI.Dtos;

public class ProductoDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public decimal PrecioBase { get; set; }
    public string Tipo { get; set; } = string.Empty;
}
