using Practica_net1_LogiTrack.Pedidos;

namespace Practica_net1_LogiTrack.Servicios
{
    // Canal por SMS (simulado para el módulo 1).
    public class NotificadorSMS : INotificador
    {
        private readonly string _telefono;

        public NotificadorSMS(string telefono) => _telefono = telefono;

        public void Enviar(string mensaje)
            => Console.WriteLine($"[SMS a {_telefono}] {mensaje}");
    }
}
