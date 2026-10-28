using Practica_net1_LogiTrack.Modelo;
using Practica_net1_LogiTrack.Usuarios;

namespace Practica_net1_LogiTrack.Pedidos
{
    public class Pedido
    {
        private static int _contador;
        private readonly List<AProducto> _productos = new();

        public int Numero { get; }
        public Cliente Cliente { get; }
        public IReadOnlyList<AProducto> Productos => _productos.AsReadOnly();
        public DateTime Fecha { get; }
        public EstadoPedido Estado { get; private set; } = EstadoPedido.Pendiente;

        private readonly IPago _pago;
        private readonly INotificador _notificador;

        public Pedido(Cliente cliente, IPago pago, INotificador notificador, DateTime? fecha = null)
        {
            Cliente = cliente ?? throw new ArgumentNullException(nameof(cliente));
            _pago = pago ?? throw new ArgumentNullException(nameof(pago));
            _notificador = notificador ?? throw new ArgumentNullException(nameof(notificador));

            Numero = ++_contador;
            Fecha = fecha ?? DateTime.Now;
        }

        public void AgregarProducto(AProducto producto)
        {
            if (producto is null)
                throw new ArgumentNullException(nameof(producto));

            _productos.Add(producto);
        }

        public void CambiarEstado(EstadoPedido nuevoEstado)
        {
            if (nuevoEstado == EstadoPedido.Pendiente && Estado != EstadoPedido.Pendiente)
                throw new InvalidOperationException("No se puede volver a Pendiente");

            Estado = nuevoEstado;
        }

        // Recorre la abstracción común: cada producto aporta su total sin if/switch.
        public decimal CalcularTotal()
        {
            decimal total = 0;
            foreach (var producto in _productos)
                total += producto.ObtenerTotalCosto();
            return total;
        }

        public string Confirmar()
        {
            decimal total = CalcularTotal();

            if (!_pago.Validar(total))
                throw new InvalidOperationException($"El pago con {_pago.Nombre} fue rechazado para el monto {total}.");

            string comprobante = _pago.Procesar(total);
            _notificador.Enviar($"Pedido #{Numero} confirmado para {Cliente.Nombre}. Total: {total:C}. Comprobante: {comprobante}.");
            Estado = EstadoPedido.Confirmado;
            return comprobante;
        }
    }
}
