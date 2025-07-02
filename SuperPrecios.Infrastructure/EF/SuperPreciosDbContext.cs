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
            //new UsuarioConfiguracion().Configure(modelBuilder.Entity<Usuario>());
            modelBuilder.ApplyConfiguration(new UsuarioConfiguracion());

            modelBuilder.Entity<Proveedor>()
            .HasOne(p => p.Supermercado)
            .WithOne(s => s.Proveedor)
            .HasForeignKey<Proveedor>(p => p.SupermercadoId)
            .IsRequired(false) // ¡IMPORTANTE para evitar error en TPH!
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proveedor>()
                .HasIndex(p => p.SupermercadoId)
                .IsUnique();

            modelBuilder.Entity<Categoria>()
            .HasOne(c => c.Padre)
            .WithMany(c => c.Hijos)
            .HasForeignKey(c => c.PadreId)
            .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
