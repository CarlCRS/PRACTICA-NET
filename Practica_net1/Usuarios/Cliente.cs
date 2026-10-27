namespace Practica_net1_LogiTrack.Usuarios
{
    public class Cliente : AUsuario
    {
        public Cliente(string nombre, string email) : base(nombre, email) { }

        public override IReadOnlyCollection<Permisos> Permisos { get; } =
            new HashSet<Permisos>
            {
                Usuarios.Permisos.VerCatalogo,
                Usuarios.Permisos.CrearPedido,
                Usuarios.Permisos.VerPedidosPropios
            };
    }
}
