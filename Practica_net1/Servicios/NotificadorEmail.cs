using Practica_net1_LogiTrack.Pedidos;

namespace Practica_net1_LogiTrack.Servicios
{
    // Canal por email (simulado para el módulo 1).
    public class NotificadorEmail : INotificador
    {
        private readonly string _destino;

        public NotificadorEmail(string destino) => _destino = destino;

        public void Enviar(string mensaje)
            => Console.WriteLine($"[EMAIL a {_destino}] {mensaje}");
    }
}
