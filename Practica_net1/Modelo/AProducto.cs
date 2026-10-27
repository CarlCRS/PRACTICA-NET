namespace Practica_net1_LogiTrack.Modelo
{
    public abstract class AProducto
    {
        private static int _contador;

        public int ID { get; }
        public string Nombre { get; }
        public decimal PrecioBase { get; }

        protected AProducto(string nombre, decimal precio)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del producto no puede estar vacío", nameof(nombre));
            if (precio <= 0)
                throw new ArgumentException("El precio del producto no puede ser menor o igual a 0", nameof(precio));

            ID = ++_contador;
            Nombre = nombre;
            PrecioBase = precio;
        }

        // Costo extra según el tipo de producto (envío, gestión, agenda...).
        public abstract decimal calcularCosto();

        public decimal ObtenerTotalCosto() => PrecioBase + calcularCosto();
    }
}
