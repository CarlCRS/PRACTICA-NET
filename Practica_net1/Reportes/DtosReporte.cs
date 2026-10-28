namespace Practica_net1_LogiTrack.Reportes
{
    // DTOs de reporte: resultados proyectados, no entidades del dominio.
    public record ClienteVentasDto(string Cliente, int Pedidos, decimal TotalComprado);

    public record IngresosTipoPeriodoDto(string Periodo, string TipoProducto, int Unidades, decimal Ingresos);

    public record InventarioBajoDto(string SKU, string Nombre, int StockInicial, int StockActual, decimal PorcentajeRestante);

    public record ClienteEstadosDto(string Cliente, List<string> Estados);

    public record ClienteRiesgoDto(string Cliente, DateTime UltimaCompra, int DiasSinComprar, decimal PromedioCompra, bool EnRiesgo);
}
