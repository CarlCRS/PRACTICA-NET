namespace LogiTrack_WebAPI.Dtos;

public class ReporteClienteRiesgoDto
{
    public string Cliente { get; set; } = string.Empty;
    public int Pedidos { get; set; }
    public decimal TotalComprado { get; set; }
    public decimal DiasSinComprar { get; set; }
    public bool EnRiesgo { get; set; }
}
