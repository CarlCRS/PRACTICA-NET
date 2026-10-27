namespace Practica_net1_LogiTrack.Pedidos
{
    // El pedido avisa eventos sin saber por qué canal.
    public interface INotificador
    {
        void Enviar(string mensaje);
    }
}
