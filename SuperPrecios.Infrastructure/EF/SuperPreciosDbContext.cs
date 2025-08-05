using SuperPrecios.AuthenticationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperPrecios.Infrastructure.Configuraciones;
using SuperPrecios.Domain.Entities;

namespace SuperPrecios.Infrastructure.EF
{
    public class SuperPreciosDbContext : DbContext
    {
        public DbSet<Supermercado> Supermercados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<PrecioHistorico> PreciosHistoricos { get; set; }

        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Miembro> Miembros { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        public SuperPreciosDbContext(DbContextOptions<SuperPreciosDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de usuarios existente
            modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());

            // ✅ CONFIGURACIÓN DE IDS EXTERNOS (Python maneja los IDs)
            modelBuilder.Entity<Marca>()
                .Property(m => m.Id)
                .ValueGeneratedNever(); // No autonumérico

            modelBuilder.Entity<Producto>()
                .Property(p => p.Id)
                .ValueGeneratedNever(); // No autonumérico

            // ✅ OPTIMIZACIONES DE PERFORMANCE

            // Índices optimizados para búsquedas frecuentes
            modelBuilder.Entity<PrecioHistorico>()
                .HasIndex(ph => new { ph.ProductoId, ph.SupermercadoId, ph.Fecha })
                .HasDatabaseName("IX_PrecioHistorico_ProductoSupermercadoFecha");

            modelBuilder.Entity<PrecioHistorico>()
                .HasIndex(ph => ph.Fecha)
                .HasDatabaseName("IX_PrecioHistorico_Fecha");

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.MarcaId)
                .HasDatabaseName("IX_Producto_MarcaId");

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.CategoriaId)
                .HasDatabaseName("IX_Producto_CategoriaId");

            // Configuración de precisión para decimales (performance en SQL Server)
            modelBuilder.Entity<PrecioHistorico>()
                .Property(ph => ph.Precio)
                .HasPrecision(18, 2);

            // ✅ CONFIGURACIONES EXISTENTES
            modelBuilder.Entity<Proveedor>()
                .HasOne(p => p.Supermercado)
                .WithOne(s => s.Proveedor)
                .HasForeignKey<Proveedor>(p => p.SupermercadoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.SupermercadoId)
                .IsUnique();

            modelBuilder.Entity<Categoria>()
                .HasOne(c => c.Padre)
                .WithMany(c => c.Hijos)
                .HasForeignKey(c => c.PadreId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ CONFIGURACIÓN ADICIONAL PARA PERFORMANCE

            // Configurar comportamiento de tracking para consultas frecuentes
            modelBuilder.Entity<PrecioHistorico>()
                .HasQueryFilter(ph => ph.Precio > 0); // Filtro global para precios válidos
        }

        // ✅ OVERRIDE PARA OPTIMIZAR SAVECHANGES EN LOTES
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Optimización: Configurar timeout más alto para operaciones en lote
            Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}