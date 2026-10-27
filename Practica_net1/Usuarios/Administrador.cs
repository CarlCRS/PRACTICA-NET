namespace Practica_net1_LogiTrack.Usuarios
{
    public class Administrador : AUsuario
    {
        public Administrador(string nombre, string email) : base(nombre, email) { }

        public override IReadOnlyCollection<Permisos> Permisos { get; } =
            new HashSet<Permisos>
            {
                Usuarios.Permisos.VerCatalogo,
                Usuarios.Permisos.CrearPedido,
                Usuarios.Permisos.VerPedidosPropios,
                Usuarios.Permisos.VerTodosLosPedidos,
                Usuarios.Permisos.GestionarStock,
                Usuarios.Permisos.PrepararPedidos,
                Usuarios.Permisos.GestionarUsuarios,
                Usuarios.Permisos.VerReportes
            };
    }
}
