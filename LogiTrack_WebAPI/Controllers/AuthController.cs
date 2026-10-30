using LogiTrack_WebAPI.Dtos;
using LogiTrack_WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiTrack_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly TokenService _token;

    public AuthController(AuthService auth, TokenService token)
    {
        _auth = auth;
        _token = token;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto dto)
    {
        var valido = _auth.Validar(dto.Username, dto.Password);
        if (valido is null)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        var (username, role) = valido.Value;

        // Creamos un Usuario ligero solo para el token
        var usuario = new Practica_net1_LogiTrack.Repositorio.Usuario
        {
            Id = 1,
            Nombre = username,
            Email = role
        };

        var token = _token.CreateToken(usuario);

        return Ok(new LoginResponseDto
        {
            Username = username,
            Role = role,
            Token = token
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequestDto dto)
    {
        if (_auth.Existe(dto.Username))
            return BadRequest(new { message = "El nombre de usuario ya existe" });

        _auth.Registrar(dto.Username, dto.Password, dto.Role);
        return Ok(new { message = "Usuario registrado con éxito" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var username = User.Identity?.Name ?? "desconocido";
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "sin rol";
        return Ok(new { Username = username, Role = role });
    }
}
