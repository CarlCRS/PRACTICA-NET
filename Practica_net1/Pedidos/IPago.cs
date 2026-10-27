namespace Practica_net1_LogiTrack.Pedidos
{
    // El pedido no conoce implementaciones concretas de pago, solo este contrato.
    public interface IPago
    {
        string Nombre { get; }
        bool Validar(decimal monto);
        string Procesar(decimal monto);
    }
}
