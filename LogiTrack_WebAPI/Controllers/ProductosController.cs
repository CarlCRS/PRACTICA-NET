using LogiTrack_WebAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practica_net1_LogiTrack.Repositorio;

namespace LogiTrack_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly EfCoreRepositorio _repo;

    public ProductosController(EfCoreRepositorio repo) => _repo = repo;

    [HttpGet]
    public IActionResult ObtenerProductos()
    {
        var productos = _repo.ObtenerProductos()
            .Select(p => new ProductoDto
            {
                Id = p.Id,
                SKU = p.SKU,
                Nombre = p.Nombre,
                PrecioBase = p.PrecioBase,
                Tipo = p.Tipo
            })
            .ToList();

        return Ok(productos);
    }
}
