using Microsoft.EntityFrameworkCore;

namespace Delf_WebApp.Models
{
    public class AppDbContext : DbContext
    {
        //Utilizacion de la dependencia de conxion.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        //Relaciones entre tablas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar columna decimal
            modelBuilder.Entity<Articulo>()
                .Property(a => a.Precio)
                .HasColumnType("decimal(18,2)");

            //Relacion uno a muchos entre Articulo y Categoria
            modelBuilder.Entity<Articulo>()
                .HasOne(a => a.Categoria)
                .WithMany(c => c.Articulos)
                .HasForeignKey(a => a.CategoriaId);

            //Relacion uno a muchos entre Pedido y Cliente
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.ClienteId);

            //Relacion uno a muchos entre Cliente y Viajante
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Viajante)
                .WithMany(v => v.Clientes)
                .HasForeignKey(c => c.ViajanteId);

            // Relacion muchos a muchos entre Pedido y Articulo a través de ArticuloCantidad
            modelBuilder.Entity<ArticuloCantidad>()
                .HasKey(ac => new { ac.PedidoId, ac.ArticuloId });

            modelBuilder.Entity<ArticuloCantidad>()
                .HasOne(ac => ac.Pedido)
                .WithMany(p => p.ArticulosCantidades)
                .HasForeignKey(ac => ac.PedidoId);

            modelBuilder.Entity<ArticuloCantidad>()
                .HasOne(ac => ac.Articulo)
                .WithMany(a => a.ArticulosCantidades)
                .HasForeignKey(ac => ac.ArticuloId);

        }

        //Creacion de tablas en la Bd
        public DbSet<Categoria>? Categorias { get; set; }
        public DbSet<Articulo>? Articulos { get; set; }
        public DbSet<Cliente>? Clientes { get; set; }
        public DbSet<Pedido>? Pedidos { get; set; }
        public DbSet<Viajante>? Viajantes { get; set; }
        public DbSet<ArticuloCantidad>? ArticuloCantidades { get; set; }        
        public DbSet<Usuario>? Usuarios { get; set; } // Agregar Usuarios
    }

    
}
