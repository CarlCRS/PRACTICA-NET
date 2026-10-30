using LogiTrack_WebAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practica_net1_LogiTrack.Repositorio;

namespace LogiTrack_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly DapperRepositorio _dapper;

    public PedidosController(DapperRepositorio dapper) => _dapper = dapper;

    [HttpPost]
    [Authorize(Policy = "Cliente")]
    public IActionResult CrearPedido([FromBody] PedidoRequestDto dto)
    {
        if (dto.Detalles == null || !dto.Detalles.Any())
            return BadRequest(new { message = "El pedido debe tener al menos un producto" });

        try
        {
            var detallesRepo = dto.Detalles.Select(d => new Practica_net1_LogiTrack.Repositorio.DetallePedidoDto
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad
            }).ToList();

            var id = _dapper.RegistrarPedido(dto.ClienteId, dto.MetodoPago, detallesRepo);
            return Ok(new { message = "Pedido creado", PedidoId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{pedidoId}/estado")]
    [Authorize(Policy = "Bodeguero")]
    public IActionResult CambiarEstado(int pedidoId, [FromBody] CambioEstadoDto dto)
    {
        var estadosValidos = new[] { "Pendiente", "Confirmado", "EnPreparacion", "Enviado", "Entregado", "Cancelado" };
        if (!estadosValidos.Contains(dto.NuevoEstado))
            return BadRequest(new { message = "Estado no válido" });

        // Aquí se podría usar un repositorio para cambiar el estado en BD
        return Ok(new { message = $"Pedido {pedidoId} cambiado a {dto.NuevoEstado}" });
    }
}
