namespace Practica_net1_LogiTrack.Modelo
{
    public class ProductoFisico : AProducto
    {
        private const decimal CostoKg = 2.5m;

        public decimal PesoKg { get; }

        public ProductoFisico(string nombre, decimal precio, decimal pesoKg) : base(nombre, precio)
        {
            if (pesoKg <= 0)
                throw new ArgumentException("El peso no puede ser 0 o inferior", nameof(pesoKg));

            PesoKg = pesoKg;
        }

        // Solo el envío; el PrecioBase lo suma ObtenerTotalCosto de la clase base.
        public override decimal calcularCosto() => PesoKg * CostoKg;
    }
}
