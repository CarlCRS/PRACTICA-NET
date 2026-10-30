using Microsoft.EntityFrameworkCore;

namespace Practica_net1_LogiTrack.Repositorio;

public class EfCoreRepositorio
{
    private readonly LogiTrackContext _db;

    public EfCoreRepositorio(LogiTrackContext db) => _db = db;

    public Usuario? ObtenerUsuarioPorNombre(string nombre) =>
        _db.Usuarios.FirstOrDefault(u => u.Nombre == nombre);

    public List<Producto> ObtenerProductos() => _db.Productos.ToList();

    public List<PedidoConDetalle> ObtenerPedidosConDetalle()
    {
        return _db.Pedidos
            .Include(p => p.Detalles)
            .OrderByDescending(p => p.Id)
            .ToList()
            .Select(p => new PedidoConDetalle
            {
                Id = p.Id,
                ClienteId = p.ClienteId,
                Estado = p.Estado,
                Total = p.Total,
                Detalles = p.Detalles.Select(d => new PedidoDetalleDto
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            })
            .ToList();
    }
}

public class PedidoConDetalle
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<PedidoDetalleDto> Detalles { get; set; } = new();
}

public class PedidoDetalleDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
