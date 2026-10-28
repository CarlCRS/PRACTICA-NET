namespace Practica_net1_LogiTrack.Modelo
{
    // Stock de un producto físico: lo que entró y lo que queda.
    public class StockProducto
    {
        public string SKU { get; }
        public string Nombre { get; }
        public int StockInicial { get; }
        public int StockActual { get; private set; }

        public StockProducto(string sku, string nombre, int stockInicial)
        {
            if (stockInicial <= 0)
                throw new ArgumentException("El stock inicial debe ser mayor a 0", nameof(stockInicial));

            SKU = sku;
            Nombre = nombre;
            StockInicial = stockInicial;
            StockActual = stockInicial;
        }

        public void Descontar(int cantidad = 1)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));
            if (StockActual < cantidad)
                throw new InvalidOperationException($"Stock insuficiente para {Nombre}");

            StockActual -= cantidad;
        }
    }

    // Inventario en memoria: búsqueda directa por SKU con Dictionary.
    public class Inventario
    {
        private readonly Dictionary<string, StockProducto> _stock = new();

        public IReadOnlyDictionary<string, StockProducto> Productos => _stock;

        public void Registrar(ProductoFisico producto, int stockInicial)
        {
            if (producto is null)
                throw new ArgumentNullException(nameof(producto));
            if (_stock.ContainsKey(producto.SKU))
                throw new InvalidOperationException($"El SKU {producto.SKU} ya está registrado en el inventario");

            _stock[producto.SKU] = new StockProducto(producto.SKU, producto.Nombre, stockInicial);
        }

        public void Descontar(ProductoFisico producto, int cantidad = 1)
        {
            if (producto is null)
                throw new ArgumentNullException(nameof(producto));
            if (!_stock.ContainsKey(producto.SKU))
                throw new InvalidOperationException($"El SKU {producto.SKU} no está registrado en el inventario");

            _stock[producto.SKU].Descontar(cantidad);
        }
    }
}
