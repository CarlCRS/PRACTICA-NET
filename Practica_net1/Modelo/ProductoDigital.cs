namespace Practica_net1_LogiTrack.Modelo
{
    public class ProductoDigital : AProducto
    {
        public string UrlDescarga { get; }

        public override TipoProducto Tipo => TipoProducto.Digital;

        public ProductoDigital(string nombre, decimal precio, string sku, string urlDescarga)
            : base(nombre, precio, sku)
        {
            if (string.IsNullOrWhiteSpace(urlDescarga))
                throw new ArgumentException("La URL de descarga no puede estar vacía", nameof(urlDescarga));

            UrlDescarga = urlDescarga;
        }

        public override decimal calcularCosto() => 0m;
    }
}
