namespace Practica_net1_LogiTrack.Modelo
{
    public class PServicio : AProducto
    {
        private const decimal TarifaAgenda = 10m;

        public DateTime FechaAgenda { get; }
        public decimal DuracionHoras { get; }

        public override TipoProducto Tipo => TipoProducto.Servicio;

        public PServicio(string nombre, decimal precio, string sku, DateTime fechaAgenda, decimal duracionHoras)
            : base(nombre, precio, sku)
        {
            if (fechaAgenda <= DateTime.Now)
                throw new ArgumentException("La fecha del servicio debe ser futura", nameof(fechaAgenda));
            if (duracionHoras <= 0)
                throw new ArgumentException("La duración debe ser mayor a 0", nameof(duracionHoras));

            FechaAgenda = fechaAgenda;
            DuracionHoras = duracionHoras;
        }

        // Tarifa fija por la gestión de agenda del servicio.
        public override decimal calcularCosto() => TarifaAgenda;
    }
}
