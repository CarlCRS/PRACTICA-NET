using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Practica_net1_LogiTrack.Repositorio;

public class DapperRepositorio
{
    private readonly string _cs =
        "Server=(localdb)\\MSSQLLocalDB;Database=LogiTrackDB;Trusted_Connection=True;";

    // Reporte: clientes con mayor monto comprado
    public List<VentaClienteDto> VentasPorCliente()
    {
        using var c = new SqlConnection(_cs);
        return c.Query<VentaClienteDto>(@"
            SELECT u.Nombre AS Cliente,
                   COUNT(p.Id) AS Pedidos,
                   SUM(p.Total) AS TotalComprado
            FROM Pedidos p
            JOIN Usuarios u ON u.Id = p.ClienteId
            WHERE p.Estado <> 'Cancelado'
            GROUP BY u.Id, u.Nombre
            ORDER BY TotalComprado DESC").ToList();
    }

    // Reporte: ingresos por tipo y periodo
    public List<IngresoDto> IngresosPorTipoYPeriodo()
    {
        using var c = new SqlConnection(_cs);
        return c.Query<IngresoDto>(@"
            SELECT CONCAT(YEAR(p.Fecha), '-', RIGHT('0' + CAST(MONTH(p.Fecha) AS VARCHAR(2)), 2)) AS Periodo,
                   pr.Tipo AS TipoProducto,
                   SUM(d.Cantidad) AS Unidades,
                   SUM(d.Cantidad * d.PrecioUnitario) AS Ingresos
            FROM Pedidos p
            JOIN PedidoDetalles d ON d.PedidoId = p.Id
            JOIN Productos pr ON pr.Id = d.ProductoId
            WHERE p.Estado <> 'Cancelado'
            GROUP BY YEAR(p.Fecha), MONTH(p.Fecha), pr.Tipo
            ORDER BY Periodo, TipoProducto").ToList();
    }

    // Consulta nativa: historial de cliente usando la función de tabla del módulo 03
    public List<HistorialDto> HistorialCliente(int clienteId)
    {
        using var c = new SqlConnection(_cs);
        return c.Query<HistorialDto>(
            "SELECT * FROM dbo.fn_HistorialCliente(@clienteId) ORDER BY Fecha",
            new { clienteId }).ToList();
    }

    // Registrar pedido completo vía stored procedure (detalles como TVP)
    public int RegistrarPedido(int clienteId, string metodoPago, List<DetallePedidoDto> detalles)
    {
        var tabla = new DataTable();
        tabla.Columns.Add("ProductoId", typeof(int));
        tabla.Columns.Add("Cantidad", typeof(int));
        foreach (var d in detalles)
            tabla.Rows.Add(d.ProductoId, d.Cantidad);

        using var c = new SqlConnection(_cs);
        using var cmd = c.CreateCommand();
        cmd.CommandText = "dbo.sp_RegistrarPedido";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ClienteId", clienteId);
        cmd.Parameters.AddWithValue("@MetodoPago", metodoPago);

        var pDet = cmd.Parameters.Add("@Detalles", SqlDbType.Structured);
        pDet.TypeName = "dbo.PedidoDetalleTipo";
        pDet.Value = tabla;

        c.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}

public class VentaClienteDto
{
    public string Cliente { get; set; } = string.Empty;
    public int Pedidos { get; set; }
    public decimal TotalComprado { get; set; }
}

public class IngresoDto
{
    public string Periodo { get; set; } = string.Empty;
    public string TipoProducto { get; set; } = string.Empty;
    public int Unidades { get; set; }
    public decimal Ingresos { get; set; }
}

public class HistorialDto
{
    public int PedidoId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
}

public class DetallePedidoDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
}
