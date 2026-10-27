namespace Practica_net1_LogiTrack.Usuarios
{
    public abstract class AUsuario
    {
        private static int _contador;

        public int ID { get; }
        public string Nombre { get; }
        public string Email { get; }

        protected AUsuario(string nombre, string email)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del usuario no puede estar vacío", nameof(nombre));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email del usuario no puede estar vacío", nameof(email));

            ID = ++_contador;
            Nombre = nombre;
            Email = email;
        }

        // Cada tipo de usuario devuelve sus permisos según su rol.
        public abstract IReadOnlyCollection<Permisos> Permisos { get; }

        public bool TienePermiso(Permisos permiso) => Permisos.Contains(permiso);
    }
}
