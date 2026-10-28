using Practica_net1_LogiTrack.Modelo;
using Practica_net1_LogiTrack.Pedidos;
using Practica_net1_LogiTrack.Usuarios;
using Practica_net1_LogiTrack.Servicios;

namespace Practica_net1_LogiTrack.Datos
{
    // Conjunto de datos en memoria compartido por los reportes.
    public class DatosEmpresa
    {
        public List<AProducto> Catalogo { get; }
        public List<AUsuario> Usuarios { get; }
        public List<Pedido> Pedidos { get; }
        public Inventario Inventario { get; }

        public DatosEmpresa(List<AProducto> catalogo, List<AUsuario> usuarios, List<Pedido> pedidos, Inventario inventario)
        {
            Catalogo = catalogo;
            Usuarios = usuarios;
            Pedidos = pedidos;
            Inventario = inventario;
        }
    }

    public static class GeneradorDatos
    {
        // Crea un pedido, lo agrega a la lista y descuenta inventario de los físicos (los cancelados no descuentan).
        private static void Crear(List<Pedido> pedidos, Inventario inventario, Cliente cliente,
            DateTime fecha, EstadoPedido estado, params AProducto[] productos)
        {
            var pedido = new Pedido(cliente, new PagoTarjeta(9_999_999m), new NotificadorEmail(cliente.Email), fecha);

            foreach (var producto in productos)
                pedido.AgregarProducto(producto);

            pedido.CambiarEstado(estado);
            pedidos.Add(pedido);

            if (estado != EstadoPedido.Cancelado)
                foreach (var fisico in productos.OfType<ProductoFisico>())
                    inventario.Descontar(fisico);
        }
        public static DatosEmpresa Generar()
        {
            // Productos del catálogo
            var notebook = new ProductoFisico("Notebook Lenovo", 850000m, "F-NOTE-01", 3.2m);
            var monitor = new ProductoFisico("Monitor 24 pulgadas", 180000m, "F-MONI-02", 5.5m);
            var teclado = new ProductoFisico("Teclado Mecánico", 45000m, "F-TECL-03", 1.2m);
            var impresora = new ProductoFisico("Impresora Térmica", 120000m, "F-IMPR-04", 2.0m);
            var licencia = new ProductoDigital("Licencia Antivirus", 12000m, "D-AV-001", "https://descargas.logitrack.com/av");
            var ebook = new ProductoDigital("Ebook C#", 3000m, "D-EBOOK-02", "https://descargas.logitrack.com/ebook-cs");
            var curso = new ProductoDigital("Curso SQL Online", 25000m, "D-CURSO-03", "https://descargas.logitrack.com/curso-sql");
            var instalacion = new PServicio("Instalación y configuración", 45000m, "S-INST-01", DateTime.Now.AddDays(15), 2m);
            var mantenimiento = new PServicio("Mantenimiento anual", 150000m, "S-MANT-02", DateTime.Now.AddDays(30), 8m);

            var catalogo = new List<AProducto>
            {
                notebook, monitor, teclado, impresora,
                licencia, ebook, curso,
                instalacion, mantenimiento
            };

            // HashSet garantiza SKUs únicos en el catálogo.
            var skus = new HashSet<string>(catalogo.Select(p => p.SKU));
            if (skus.Count != catalogo.Count)
                throw new InvalidOperationException("Hay SKUs duplicados en el catálogo");

            // Inventario: solo los productos físicos lo requieren.
            var inventario = new Inventario();
            inventario.Registrar(notebook, 10);
            inventario.Registrar(monitor, 6);
            inventario.Registrar(teclado, 6);
            inventario.Registrar(impresora, 25);

            // Usuarios
            var maria = new Cliente("María Gómez", "maria@mail.com");
            var carlos = new Cliente("Carlos Ruiz", "carlos@mail.com");
            var lucia = new Cliente("Lucía Fernández", "lucia@mail.com");
            var diego = new Cliente("Diego Torres", "diego@mail.com");
            var sofia = new Cliente("Sofía Méndez", "sofia@mail.com");
            var bodeguero = new Bodeguero("Juan Pérez", "juan.bodega@logitrack.com");
            var admin = new Administrador("Ana Sosa", "ana.admin@logitrack.com");

            var usuarios = new List<AUsuario> { maria, carlos, lucia, diego, sofia, bodeguero, admin };

            // Pedidos con fechas, estados y productos variados.
            var pedidos = new List<Pedido>();
            var hoy = DateTime.Today;

            Crear(pedidos, inventario, maria, hoy.AddDays(-160), EstadoPedido.Entregado, notebook, licencia);
            Crear(pedidos, inventario, carlos, hoy.AddDays(-150), EstadoPedido.Entregado, monitor, teclado);
            Crear(pedidos, inventario, maria, hoy.AddDays(-120), EstadoPedido.Entregado, curso, ebook);
            Crear(pedidos, inventario, lucia, hoy.AddDays(-110), EstadoPedido.Enviado, mantenimiento);
            Crear(pedidos, inventario, carlos, hoy.AddDays(-95), EstadoPedido.Enviado, notebook);
            Crear(pedidos, inventario, diego, hoy.AddDays(-80), EstadoPedido.Entregado, impresora, licencia);
            Crear(pedidos, inventario, maria, hoy.AddDays(-60), EstadoPedido.Entregado, teclado);
            Crear(pedidos, inventario, lucia, hoy.AddDays(-50), EstadoPedido.Confirmado, monitor, ebook);
            Crear(pedidos, inventario, sofia, hoy.AddDays(-75), EstadoPedido.Entregado, teclado, licencia);
            Crear(pedidos, inventario, sofia, hoy.AddDays(-72), EstadoPedido.Pendiente, ebook);
            Crear(pedidos, inventario, diego, hoy.AddDays(-40), EstadoPedido.Cancelado, curso);
            Crear(pedidos, inventario, carlos, hoy.AddDays(-30), EstadoPedido.Cancelado, notebook, teclado);
            Crear(pedidos, inventario, maria, hoy.AddDays(-25), EstadoPedido.Entregado, monitor);
            Crear(pedidos, inventario, carlos, hoy.AddDays(-20), EstadoPedido.Confirmado, monitor, curso);
            Crear(pedidos, inventario, diego, hoy.AddDays(-15), EstadoPedido.Confirmado, teclado);
            Crear(pedidos, inventario, carlos, hoy.AddDays(-8), EstadoPedido.Pendiente, teclado, impresora);
            Crear(pedidos, inventario, lucia, hoy.AddDays(-5), EstadoPedido.EnPreparacion, impresora);

            return new DatosEmpresa(catalogo, usuarios, pedidos, inventario);
        }

        
    }
}
