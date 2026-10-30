using LogiTrack_WebAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practica_net1_LogiTrack.Repositorio;

namespace LogiTrack_WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Admin")]
public class ReportesController : ControllerBase
{
    private readonly DapperRepositorio _dapper;

    public ReportesController(DapperRepositorio dapper) => _dapper = dapper;

    [HttpGet("ventas-clientes")]
    public IActionResult VentasPorCliente()
    {
        var ventas = _dapper.VentasPorCliente();
        return Ok(ventas);
    }

    [HttpGet("ingresos-tipo-periodo")]
    public IActionResult IngresosPorTipoYPeriodo()
    {
        var ingresos = _dapper.IngresosPorTipoYPeriodo();
        return Ok(ingresos);
    }

    [HttpGet("clientes-riesgo")]
    public IActionResult ClientesEnRiesgo()
    {
        // Usa el reporte de ventas como base y marca riesgo por promedio bajo
        var ventas = _dapper.VentasPorCliente();
        var riesgo = ventas.Select(v => new ReporteClienteRiesgoDto
        {
            Cliente = v.Cliente,
            Pedidos = v.Pedidos,
            TotalComprado = v.TotalComprado,
            DiasSinComprar = 0,
            EnRiesgo = v.TotalComprado < 500000
        }).ToList();

        return Ok(riesgo);
    }
}
