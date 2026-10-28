namespace Practica_net1_LogiTrack.Repositorio;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RolId { get; set; }
    public DateTime FechaAlta { get; set; }
}
