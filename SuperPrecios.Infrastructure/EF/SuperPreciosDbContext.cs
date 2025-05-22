using SuperPrecios.AutenticacionCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperPrecios.Infrastructure.Configuraciones;
using SuperPrecios.Domain.Entidades;

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

        public SuperPreciosDbContext(DbContextOptions<SuperPreciosDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new UsuarioConfiguracion().Configure(modelBuilder.Entity<Usuario>());

            modelBuilder.Entity<PrecioHistorico>()
                .HasOne(ph => ph.Producto)
                .WithMany()
                .HasForeignKey(ph => ph.ProductoId)
                .OnDelete(DeleteBehavior.Restrict); // o NoAction

            modelBuilder.Entity<PrecioHistorico>()
                .HasOne(ph => ph.Supermercado)
                .WithMany()
                .HasForeignKey(ph => ph.SupermercadoId)
                .OnDelete(DeleteBehavior.Restrict); // o NoAction
        }

    }
}
