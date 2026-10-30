using LogiTrack_WebAPI.Dtos;
using Practica_net1_LogiTrack.Repositorio;

namespace LogiTrack_WebAPI.Services;

public class AuthService
{
    // Usuarios en memoria para la práctica (username, password, rol)
    private static readonly List<(string Username, string Password, string Role)> Usuarios = new()
    {
        ("carlos", "1234", "Administrador"),
        ("maria", "1234", "Cliente"),
        ("bodega", "1234", "Bodeguero")
    };

    public (string Username, string Role)? Validar(string username, string password)
    {
        var u = Usuarios.FirstOrDefault(x => x.Username == username && x.Password == password);
        return u == default ? null : (u.Username, u.Role);
    }

    public bool Existe(string username) =>
        Usuarios.Any(x => x.Username == username);

    public void Registrar(string username, string password, string role)
    {
        Usuarios.Add((username, password, role));
    }
}
