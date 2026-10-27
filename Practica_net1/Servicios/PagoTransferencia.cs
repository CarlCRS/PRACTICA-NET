using Practica_net1_LogiTrack.Pedidos;

namespace Practica_net1_LogiTrack.Servicios
{
    // Transferencia con monto mínimo (uso B2B, no compras chicas).
    public class PagoTransferencia : IPago
    {
        private const decimal MontoMinimo = 50000m;

        public string Nombre => "Transferencia";

        public bool Validar(decimal monto) => monto >= MontoMinimo;

        public string Procesar(decimal monto)
            => $"TRANSF-{Guid.NewGuid():N}".ToUpper()[..18];
    }
}
