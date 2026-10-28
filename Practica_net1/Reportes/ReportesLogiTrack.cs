using Practica_net1_LogiTrack.Datos;
using Practica_net1_LogiTrack.Modelo;
using Practica_net1_LogiTrack.Pedidos;

namespace Practica_net1_LogiTrack.Reportes
{
    // Reportes de negocio con LINQ sobre los datos en memoria.
    public class ReportesLogiTrack
    {
        private readonly DatosEmpresa _datos;

        public ReportesLogiTrack(DatosEmpresa datos) => _datos = datos;

        // 1. Agrupación por cliente + agregación de montos.
        public List<ClienteVentasDto> VentasPorCliente()
            => _datos.Pedidos
                .Where(p => p.Estado != EstadoPedido.Cancelado)
                .GroupBy(p => p.Cliente.Nombre)
                .Select(g => new ClienteVentasDto(g.Key, g.Count(), g.Sum(p => p.CalcularTotal())))
                .OrderByDescending(x => x.TotalComprado)
                .ToList();

        // 2. Agrupación compuesta: periodo (año-mes) × tipo de producto.
        public List<IngresosTipoPeriodoDto> IngresosPorTipoYPeriodo()
            => _datos.Pedidos
                .Where(p => p.Estado != EstadoPedido.Cancelado)
                .SelectMany(p => p.Productos, (pedido, producto) => new { pedido.Fecha, producto })
                .GroupBy(x => new { Periodo = x.Fecha.ToString("yyyy-MM"), Tipo = x.producto.Tipo })
                .Select(g => new IngresosTipoPeriodoDto(
                    g.Key.Periodo,
                    g.Key.Tipo.ToString(),
                    g.Count(),
                    g.Sum(x => x.producto.ObtenerTotalCosto())))
                .OrderBy(x => x.Periodo)
                .ThenBy(x => x.TipoProducto)
                .ToList();

        // 3. Inventario bajo: stock actual por debajo del 50% del inicial.
        public List<InventarioBajoDto> InventarioBajo()
            => _datos.Inventario.Productos.Values
                .Where(s => s.StockActual < s.StockInicial * 0.5m)
                .Select(s => new InventarioBajoDto(
                    s.SKU,
                    s.Nombre,
                    s.StockInicial,
                    s.StockActual,
                    Math.Round(100m * s.StockActual / s.StockInicial, 1)))
                .OrderBy(x => x.PorcentajeRestante)
                .ToList();

        // 4. Clientes con pedidos en más de un estado (conteo de estados distintos por cliente).
        public List<ClienteEstadosDto> ClientesConVariosEstados()
            => _datos.Pedidos
                .GroupBy(p => p.Cliente.Nombre)
                .Where(g => g.Select(p => p.Estado).Distinct().Count() > 1)
                .Select(g => new ClienteEstadosDto(
                    g.Key,
                    g.Select(p => p.Estado.ToString()).Distinct().OrderBy(e => e).ToList()))
                .OrderByDescending(x => x.Estados.Count)
                .ToList();

        // 5. Riesgo comercial: mucha distancia de la última compra o promedio de compra bajo.
        public List<ClienteRiesgoDto> ClientesEnRiesgo(int diasSinComprar = 60, decimal promedioMinimo = 30000m)
            => _datos.Pedidos
                .Where(p => p.Estado != EstadoPedido.Cancelado)
                .GroupBy(p => p.Cliente.Nombre)
                .Select(g => new
                {
                    Cliente = g.Key,
                    UltimaCompra = g.Max(p => p.Fecha),
                    Promedio = g.Average(p => p.CalcularTotal())
                })
                .Select(x => new ClienteRiesgoDto(
                    x.Cliente,
                    x.UltimaCompra,
                    (DateTime.Today - x.UltimaCompra).Days,
                    Math.Round(x.Promedio, 2),
                    (DateTime.Today - x.UltimaCompra).Days > diasSinComprar || x.Promedio < promedioMinimo))
                .Where(x => x.EnRiesgo)
                .OrderByDescending(x => x.DiasSinComprar)
                .ToList();
    }
}
