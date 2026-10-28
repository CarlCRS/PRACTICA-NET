using Practica_net1_LogiTrack.Datos;
using Practica_net1_LogiTrack.Reportes;

Console.WriteLine("=============================================");
Console.WriteLine("  LogiTrack - Reportes de negocio (Módulo 02)");
Console.WriteLine("=============================================\n");

var datos = GeneradorDatos.Generar();
var reportes = new ReportesLogiTrack(datos);

Console.WriteLine($"Catálogo: {datos.Catalogo.Count} productos | Usuarios: {datos.Usuarios.Count} | Pedidos: {datos.Pedidos.Count} | Inventario: {datos.Inventario.Productos.Count} SKUs");

// 1) Clientes con mayor monto comprado
Console.WriteLine("\n--- 1) Clientes con mayor monto comprado ---");
foreach (var r in reportes.VentasPorCliente())
    Console.WriteLine($"   {r.Cliente}: {r.TotalComprado:C} en {r.Pedidos} pedidos");

// 2) Ingresos por tipo de producto y periodo
Console.WriteLine("\n--- 2) Ingresos por tipo de producto y periodo ---");
foreach (var r in reportes.IngresosPorTipoYPeriodo())
    Console.WriteLine($"   {r.Periodo} | {r.TipoProducto,-9} | {r.Unidades} unidades | {r.Ingresos:C}");

// 3) Productos con inventario bajo
Console.WriteLine("\n--- 3) Productos con inventario bajo (< 50% del stock inicial) ---");
var bajo = reportes.InventarioBajo();
if (bajo.Count == 0)
    Console.WriteLine("   Ningún producto por debajo del umbral");
foreach (var r in bajo)
    Console.WriteLine($"   [{r.SKU}] {r.Nombre}: {r.StockActual}/{r.StockInicial} ({r.PorcentajeRestante}%)");

// 4) Clientes con pedidos en más de un estado
Console.WriteLine("\n--- 4) Clientes con pedidos en más de un estado ---");
foreach (var r in reportes.ClientesConVariosEstados())
    Console.WriteLine($"   {r.Cliente}: {string.Join(", ", r.Estados)}");

// 5) Clientes en riesgo comercial
Console.WriteLine("\n--- 5) Clientes en riesgo (sin comprar > 60 días o promedio < $30.000) ---");
var riesgo = reportes.ClientesEnRiesgo();
if (riesgo.Count == 0)
    Console.WriteLine("   Ningún cliente en riesgo");
foreach (var r in riesgo)
    Console.WriteLine($"   {r.Cliente}: última compra {r.UltimaCompra:dd/MM/yyyy} ({r.DiasSinComprar} días) | promedio {r.PromedioCompra:C}");

Console.WriteLine("\n=============================================");
Console.WriteLine("  Reportes finalizados.");
Console.WriteLine("=============================================");
