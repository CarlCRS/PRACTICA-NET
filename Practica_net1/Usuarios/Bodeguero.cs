namespace Practica_net1_LogiTrack.Usuarios
{
    public class Bodeguero : AUsuario
    {
        public Bodeguero(string nombre, string email) : base(nombre, email) { }

        public override IReadOnlyCollection<Permisos> Permisos { get; } =
            new HashSet<Permisos>
            {
                Usuarios.Permisos.VerCatalogo,
                Usuarios.Permisos.GestionarStock,
                Usuarios.Permisos.PrepararPedidos,
                Usuarios.Permisos.VerTodosLosPedidos
            };
    }
}
