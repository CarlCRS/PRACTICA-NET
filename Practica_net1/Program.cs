using Practica_net1_LogiTrack.Modelo;
using Practica_net1_LogiTrack.Pedidos;
using Practica_net1_LogiTrack.Usuarios;
using Practica_net1_LogiTrack.Servicios;

Console.WriteLine("=============================================");
Console.WriteLine("  LogiTrack - Demo dominio (Módulo 01)");
Console.WriteLine("=============================================\n");

// 1) Productos de los 3 tipos
Console.WriteLine("--- 1) Productos (3 tipos) ---");

var productos = new List<AProducto>
{
    new ProductoFisico("Notebook Lenovo", 850000m, 3.2m),
    new ProductoDigital("Licencia Antivirus", 12000m, "https://descargas.logitrack.com/av-123"),
    new PServicio("Instalación y configuración", 45000m, DateTime.Now.AddDays(3), 2m)
};

foreach (var producto in productos)
    Console.WriteLine($"[{producto.ID}] {producto.Nombre} -> Total: {producto.ObtenerTotalCosto():C}");

// 2) Permisos por rol
Console.WriteLine("\n--- 2) Usuarios y permisos por rol ---");

AUsuario cliente = new Cliente("María Gómez", "maria@mail.com");
AUsuario bodeguero = new Bodeguero("Juan Pérez", "juan.bodega@logitrack.com");
AUsuario admin = new Administrador("Ana Sosa", "ana.admin@logitrack.com");

VerPermisos(cliente, Permisos.CrearPedido, Permisos.GestionarStock);
VerPermisos(bodeguero, Permisos.GestionarStock, Permisos.CrearPedido);
VerPermisos(admin, Permisos.VerReportes, Permisos.VerPedidosPropios);

// 3) Pedido con productos de distintos tipos, pago y notificación
Console.WriteLine("\n--- 3) Pedido (mezcla de tipos + total + pago + notificación) ---");

var pedido = new Pedido(
    cliente: (Cliente)cliente,
    pago: new PagoTarjeta(limitePorOperacion: 9000000m),
    notificador: new NotificadorEmail("maria@mail.com"));

foreach (var producto in productos)
    pedido.AgregarProducto(producto);

Console.WriteLine($"Productos en el pedido #{pedido.Numero}: {pedido.Productos.Count}");
Console.WriteLine($"Total del pedido: {pedido.CalcularTotal():C}");
string comprobante = pedido.Confirmar();
Console.WriteLine($"Comprobante generado: {comprobante}");

// 4) Pago rechazado con otra implementación de pago
Console.WriteLine("\n--- 4) Pedido con pago rechazado (transferencia + monto mínimo) ---");

var pedidoChico = new Pedido(
    cliente: (Cliente)cliente,
    pago: new PagoTransferencia(),
    notificador: new NotificadorSMS("+54 9 11 5555-0000"));

pedidoChico.AgregarProducto(new ProductoDigital("Ebook C#", 3000m, "https://descargas.logitrack.com/ebook-cs"));

try
{
    pedidoChico.Confirmar();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Pago rechazado correctamente -> {ex.Message}");
}

Console.WriteLine("\n=============================================");
Console.WriteLine("  Demo finalizada sin errores de dominio.");
Console.WriteLine("=============================================");

static void VerPermisos(AUsuario usuario, params Permisos[] aProbar)
{
    Console.WriteLine($"{usuario.GetType().Name} ({usuario.Nombre}): tiene {usuario.Permisos.Count} permisos.");
    foreach (var permiso in aProbar)
        Console.WriteLine($"   ¿{permiso}? -> {(usuario.TienePermiso(permiso) ? "SÍ" : "NO")}");
}
