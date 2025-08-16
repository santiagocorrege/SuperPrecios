using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Application.IServices.PrecioHistorico;
using SuperPrecios.Application.IServices.Usuario;
using SuperPrecios.Application.IServices.Supermercado;
using SuperPrecios.Application.Services.Miembro;
using SuperPrecios.Application.Services.PrecioHistorico;
using SuperPrecios.Application.Services.Usuario;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Infrastructure.EF;
using SuperPrecios.Application.Services.Supermercado;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Application.Services.Categoria;
using SuperPrecios.Application.IServices.Matcher;
using SuperPrecios.Application.Services;

namespace SuperPrecios.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("ConexionBD");
            builder.Services.AddDbContext<SuperPreciosDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    // Agregar retry para operaciones transitorias
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            });

            //IoC : DI
            //************DI: Repositories************
            builder.Services.AddScoped<IProductoRepository, ProductoRepositoryEF>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepositoryEF>();
            builder.Services.AddScoped<IMarcaRepository, MarcaRepositoryEF>();
            builder.Services.AddScoped<ISupermercadoRepository, SupermercadoRepositoryEF>();
            builder.Services.AddScoped<IPrecioHistoricoRepository, PrecioHistoricoRepositoryEF>();
            builder.Services.AddScoped<IMatchingRepository, MatchingRepositoryEF>();

            //************DI: Services************
            //PrecioHistorico
            builder.Services.AddScoped<IPrecioHistoricoAddService, PrecioHistoricoAddService>();
            builder.Services.AddScoped<IPrecioHistoricoGetService, PrecioHistoricoGetService>();
            //Supermercado
            builder.Services.AddScoped<ISupermercadoGetService, SupermercadoGetService>();
            //Categoria
            builder.Services.AddScoped<ICategoriaGetService, CategoriaGetService>();
            builder.Services.AddScoped<ICategoriaRutaService, CategoriaRutaService>();

            //Matcher
            builder.Services.AddScoped<IMatchingProcessService, MatchingProcessService>();

            var app = builder.Build();

            // ✅ **CONFIGURACIÓN DE BASE DE DATOS Y MIGRACIONES**
            await InitializeDatabaseAsync(app);

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            //Accptar redireccion
            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        /// <summary>
        /// Inicializa la base de datos con retry logic y migraciones automáticas
        /// </summary>
        private static async Task InitializeDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SuperPreciosDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Configuración desde variables de entorno
            var recreateDb = configuration.GetValue<bool>("RECREATE_DB", false);
            var isDevelopment = app.Environment.IsDevelopment();

            logger.LogInformation("🚀 Iniciando configuración de base de datos...");
            logger.LogInformation($"   Ambiente: {app.Environment.EnvironmentName}");
            logger.LogInformation($"   Recrear BD: {recreateDb}");

            try
            {
                // **PASO 1: Esperar a que SQL Server esté listo**
                await WaitForSqlServerAsync(context, logger);

                // **PASO 2: Manejar recreación de BD (para testing)**
                if (recreateDb || (isDevelopment && recreateDb))
                {
                    logger.LogWarning("⚠️  RECREANDO base de datos...");
                    await context.Database.EnsureDeletedAsync();
                    logger.LogInformation("✅ Base de datos eliminada");
                }

                // **PASO 3: Aplicar migraciones**
                await ApplyMigrationsAsync(context, logger);

                logger.LogInformation("🎉 Base de datos configurada correctamente!");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error fatal configurando base de datos");
                throw; // Re-throw para que la aplicación no inicie mal configurada
            }
        }

        /// <summary>
        /// Espera a que SQL Server esté disponible con retry logic
        /// </summary>
        private static async Task WaitForSqlServerAsync(SuperPreciosDbContext context, ILogger logger)
        {
            var maxRetries = 30; // 30 intentos = ~90 segundos máximo
            var delay = TimeSpan.FromSeconds(3);

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    logger.LogInformation($"🔄 Intento {attempt}/{maxRetries}: Verificando conexión a SQL Server...");

                    // Intento simple de conexión
                    await context.Database.CanConnectAsync();

                    logger.LogInformation("✅ SQL Server está disponible!");
                    return;
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    logger.LogWarning($"⏳ Intento {attempt} falló: {ex.Message}");
                    logger.LogInformation($"   Reintentando en {delay.TotalSeconds} segundos...");
                    await Task.Delay(delay);
                }
            }

            throw new InvalidOperationException($"❌ No se pudo conectar a SQL Server después de {maxRetries} intentos");
        }

        /// <summary>
        /// Aplica migraciones pendientes con manejo de errores
        /// </summary>
        private static async Task ApplyMigrationsAsync(SuperPreciosDbContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("🔍 Verificando migraciones pendientes...");

                // Obtener migraciones pendientes
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();

                logger.LogInformation($"   Migraciones aplicadas: {appliedMigrations.Count()}");
                logger.LogInformation($"   Migraciones pendientes: {pendingMigrations.Count()}");

                if (pendingMigrations.Any())
                {
                    logger.LogInformation("📦 Aplicando migraciones pendientes:");
                    foreach (var migration in pendingMigrations)
                    {
                        logger.LogInformation($"   → {migration}");
                    }

                    // Aplicar migraciones
                    await context.Database.MigrateAsync();
                    logger.LogInformation("✅ Migraciones aplicadas correctamente!");
                }
                else
                {
                    logger.LogInformation("✅ Base de datos está actualizada (no hay migraciones pendientes)");
                }

                // Verificar que la BD esté operativa
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    throw new InvalidOperationException("La base de datos no responde después de las migraciones");
                }

                logger.LogInformation("✅ Base de datos verificada y operativa");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error aplicando migraciones");
                throw;
            }
        }
    }
}