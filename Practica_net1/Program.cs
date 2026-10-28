using Microsoft.EntityFrameworkCore;
using Practica_net1_LogiTrack.Repositorio;

Console.WriteLine("=============================================");
Console.WriteLine("  LogiTrack - EF Core + Dapper + Nativo (Módulo 04)");
Console.WriteLine("=============================================\n");

var opciones = new DbContextOptionsBuilder<LogiTrackContext>()
    .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LogiTrackDB;Trusted_Connection=True;")
    .Options;

using var db = new LogiTrackContext(opciones);
var ef = new EfCoreRepositorio(db);
var dap = new DapperRepositorio();

// EF Core: pedidos con detalle (sin N+1 gracias a Include)
Console.WriteLine("--- EF Core: pedidos con detalle ---");
foreach (var p in ef.ObtenerPedidosConDetalle().Take(3))
{
    Console.WriteLine($"   Pedido #{p.Id} | Estado {p.Estado} | Total {p.Total:C}");
    foreach (var d in p.Detalles)
        Console.WriteLine($"      - Producto {d.ProductoId} x{d.Cantidad} = {d.Cantidad * d.PrecioUnitario:C}");
}

// Dapper: reporte de ventas por cliente
Console.WriteLine("\n--- Dapper: clientes con mayor monto comprado ---");
foreach (var v in dap.VentasPorCliente().Take(3))
    Console.WriteLine($"   {v.Cliente}: {v.TotalComprado:C} en {v.Pedidos} pedidos");

// Dapper: ingresos por tipo y periodo
Console.WriteLine("\n--- Dapper: ingresos por tipo y periodo ---");
foreach (var i in dap.IngresosPorTipoYPeriodo().Take(4))
    Console.WriteLine($"   {i.Periodo} | {i.TipoProducto,-9} | {i.Unidades} uds | {i.Ingresos:C}");

// Consulta nativa: historial de cliente con función de tabla del módulo 03
Console.WriteLine("\n--- Consulta nativa (fn_HistorialCliente) cliente 1 ---");
foreach (var h in dap.HistorialCliente(1).Take(3))
    Console.WriteLine($"   Pedido {h.PedidoId} | {h.Fecha:dd/MM/yyyy} | {h.Estado} | {h.Total:C}");

// SP: registrar pedido completo con detalles (TVP)
Console.WriteLine("\n--- SP sp_RegistrarPedido (nuevo pedido para cliente 1) ---");
var id = dap.RegistrarPedido(1, "Tarjeta", new List<DetallePedidoDto>
{
    new DetallePedidoDto { ProductoId = 5, Cantidad = 1 },
    new DetallePedidoDto { ProductoId = 6, Cantidad = 2 }
});
Console.WriteLine($"   Pedido creado: #{id}");

Console.WriteLine("\n=============================================");
Console.WriteLine("  Módulo 04 completado.");
Console.WriteLine("=============================================");
