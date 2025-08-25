using Microsoft.EntityFrameworkCore;
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
        
        public DbSet<Carrito> Carritos { get; set; }

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

            // ✅ OPTIMIZACIONES DE PERFORMANCE PARA MATCHING

            // Índice compuesto principal para búsquedas de precios históricos
            modelBuilder.Entity<PrecioHistorico>()
                .HasIndex(ph => new { ph.ProductoId, ph.SupermercadoId, ph.Fecha })
                .HasDatabaseName("IX_PrecioHistorico_ProductoSupermercadoFecha");

            // Índice para consultas por fecha (muy frecuentes)
            modelBuilder.Entity<PrecioHistorico>()
                .HasIndex(ph => ph.Fecha)
                .HasDatabaseName("IX_PrecioHistorico_Fecha");

            // Índice para consultas por supermercado
            modelBuilder.Entity<PrecioHistorico>()
                .HasIndex(ph => ph.SupermercadoId)
                .HasDatabaseName("IX_PrecioHistorico_SupermercadoId");

            // Índices para productos (muy usados en joins)
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.MarcaId)
                .HasDatabaseName("IX_Producto_MarcaId");

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.CategoriaId)
                .HasDatabaseName("IX_Producto_CategoriaId");

            // Índice compuesto para búsquedas de productos únicos
            modelBuilder.Entity<Producto>()
                .HasIndex(p => new { p.Nombre, p.MarcaId })
                .HasDatabaseName("IX_Producto_NombreMarca")
                .IsUnique();

            // ✅ CONFIGURACIÓN DE PRECISIÓN Y PERFORMANCE

            // Configuración de precisión para decimales (performance en SQL Server)
            modelBuilder.Entity<PrecioHistorico>()
                .Property(ph => ph.Precio)
                .HasPrecision(18, 2);

            // Configuración de longitudes específicas para mejorar performance
            modelBuilder.Entity<Marca>()
                .Property(m => m.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Producto>()
                .Property(p => p.Nombre)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Producto>()
                .Property(p => p.ImgUrl)
                .HasMaxLength(500);

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            // ✅ CONFIGURACIONES EXISTENTES MEJORADAS
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

            // ✅ CONFIGURACIONES DE RELACIONES PARA PERFORMANCE

            // Configurar relaciones para evitar lazy loading en operaciones batch
            modelBuilder.Entity<PrecioHistorico>()
                .HasOne(ph => ph.Producto)
                .WithMany(p => p.PreciosHistoricos)
                .HasForeignKey(ph => ph.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PrecioHistorico>()
                .HasOne(ph => ph.Supermercado)
                .WithMany()
                .HasForeignKey(ph => ph.SupermercadoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Marca)
                .WithMany(m => m.Productos)
                .HasForeignKey(p => p.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        // ✅ OVERRIDE OPTIMIZADO PARA SAVECHANGES
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // ✅ CONFIGURACIÓN PARA OPERACIONES MASIVAS

            // Configurar timeout extendido
            Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log del error para debugging en operaciones masivas
                Console.WriteLine($"[ERROR] Error en SaveChangesAsync: {ex.Message}");
                throw;
            }
        }

        // ✅ MÉTODO AUXILIAR: Configurar contexto para operaciones de alto volumen
        public void ConfigureForBulkOperations()
        {
            // Deshabilitar AutoDetectChanges para operaciones masivas
            ChangeTracker.AutoDetectChangesEnabled = false;

            // Deshabilitar LazyLoading temporalmente
            ChangeTracker.LazyLoadingEnabled = false;

            // Configurar timeout extendido
            Database.SetCommandTimeout(TimeSpan.FromMinutes(15));

            Console.WriteLine("[INFO] Contexto configurado para operaciones masivas");
        }

        // ✅ MÉTODO AUXILIAR: Restaurar configuración normal
        public void RestoreNormalConfiguration()
        {
            // Rehabilitar configuraciones normales
            ChangeTracker.AutoDetectChangesEnabled = true;
            ChangeTracker.LazyLoadingEnabled = true;

            // Restaurar timeout normal
            Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

            Console.WriteLine("[INFO] Contexto restaurado a configuración normal");
        }

        // ✅ NUEVO MÉTODO: Ejecutar operación con configuración optimizada
        public async Task ExecuteWithBulkConfigurationAsync(Func<Task> operation)
        {
            // Configurar para operaciones masivas
            ConfigureForBulkOperations();

            try
            {
                await operation();
            }
            finally
            {
                // Siempre restaurar configuración normal
                RestoreNormalConfiguration();
            }
        }

        // ✅ MÉTODO DE DIAGNÓSTICO: Obtener estadísticas del contexto
        public object GetContextStatistics()
        {
            var entries = ChangeTracker.Entries().ToList();

            return new
            {
                TotalEntities = entries.Count,
                Added = entries.Count(e => e.State == EntityState.Added),
                Modified = entries.Count(e => e.State == EntityState.Modified),
                Deleted = entries.Count(e => e.State == EntityState.Deleted),
                Unchanged = entries.Count(e => e.State == EntityState.Unchanged),
                AutoDetectChangesEnabled = ChangeTracker.AutoDetectChangesEnabled,
                LazyLoadingEnabled = ChangeTracker.LazyLoadingEnabled
            };
        }
    }
}