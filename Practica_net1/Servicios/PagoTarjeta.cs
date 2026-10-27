using Practica_net1_LogiTrack.Pedidos;

namespace Practica_net1_LogiTrack.Servicios
{
    // Tarjeta con límite por operación.
    public class PagoTarjeta : IPago
    {
        private readonly decimal _limitePorOperacion;

        public string Nombre => "Tarjeta";

        public PagoTarjeta(decimal limitePorOperacion)
        {
            _limitePorOperacion = limitePorOperacion;
        }

        public bool Validar(decimal monto) => monto > 0 && monto <= _limitePorOperacion;

        public string Procesar(decimal monto)
            => $"COMPRA-{Guid.NewGuid():N}".ToUpper()[..18];
    }
}
