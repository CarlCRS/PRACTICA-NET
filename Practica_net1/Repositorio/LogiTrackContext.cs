using Microsoft.EntityFrameworkCore;

namespace Practica_net1_LogiTrack.Repositorio;

public class LogiTrackContext : DbContext
{
    public LogiTrackContext(DbContextOptions<LogiTrackContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoDetalle> Detalles => Set<PedidoDetalle>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Producto>(e =>
        {
            e.ToTable("Productos");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).ValueGeneratedOnAdd();
            e.Property(p => p.SKU).HasMaxLength(20).IsRequired();
            e.HasIndex(p => p.SKU).IsUnique();
            e.Property(p => p.Nombre).HasMaxLength(120).IsRequired();
            e.Property(p => p.PrecioBase).HasColumnType("DECIMAL(12,2)").IsRequired();
            e.Property(p => p.Tipo).HasMaxLength(10).IsRequired();
        });

        b.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).ValueGeneratedOnAdd();
            e.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(150).IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
        });

        b.Entity<Pedido>(e =>
        {
            e.ToTable("Pedidos");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).ValueGeneratedOnAdd();
            e.Property(p => p.ClienteId).IsRequired();
            e.Property(p => p.Estado).HasMaxLength(15).IsRequired();
            e.Property(p => p.Total).HasColumnType("DECIMAL(12,2)").IsRequired();
        });

        b.Entity<PedidoDetalle>(e =>
        {
            e.ToTable("PedidoDetalles");
            e.HasKey(d => d.Id);
            e.Property(d => d.Id).ValueGeneratedOnAdd();
            e.Property(d => d.PedidoId).IsRequired();
            e.Property(d => d.ProductoId).IsRequired();
            e.Property(d => d.Cantidad).IsRequired();
            e.Property(d => d.PrecioUnitario).HasColumnType("DECIMAL(12,2)").IsRequired();
        });
    }
}
