using SuperPrecios.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Application.Services.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Usuario;
using SuperPrecios.Application.Services.Usuario;
using Microsoft.Extensions.Options;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Application.IServices.PrecioHistorico;
using SuperPrecios.Application.Services.PrecioHistorico;

namespace SuperPrecios.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString = builder.Configuration.GetConnectionString("ConexionBD");
            builder.Services.AddDbContext<SuperPreciosDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });            

            builder.Services.AddSession();

            //IoC : DI
            //DI: Repositories
            builder.Services.AddScoped<IMiembroRepository, MiembroRepositoryEF>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryEF>();
            builder.Services.AddScoped<IProductoRepository, ProductoRepositoryEF>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepositoryEF>();
            builder.Services.AddScoped<IMarcaRepository, MarcaRepositoryEF>();
            builder.Services.AddScoped<ISupermercadoRepository, SupermercadoRepositoryEF>();
            builder.Services.AddScoped<IPrecioHistoricoRepository, PrecioHistoricoRepositoryEF>();


            //DI: Services
            //Usuario
            builder.Services.AddScoped<IUsuarioLoginService, UsuarioLogginService>();
            //Miembro
            builder.Services.AddScoped<IMiembroGet, MiembroGetService>();
            builder.Services.AddScoped<IMiembroAddService, MiembroAddService>();
            builder.Services.AddScoped<IMiembroUpdateService, MiembroUpdateService>();
            builder.Services.AddScoped<IMiembroDeleteService, MiembroDeleteService>();
            //PrecioHistorico
            builder.Services.AddScoped<IPrecioHistoricoAddService, PrecioHistoricoAddService>();
                        
            //Inversion??
            var app = builder.Build();
            //Para que se apliquen migraciones?
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SuperPreciosDbContext>();
                var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                if (environment.IsProduction())
                {
                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error applying migrations: {ex.Message}");
                        // Opcional: throw; para que el app falle si querés
                    }
                }
            }

            Console.WriteLine("Cadena de conexión usada: " + connectionString);

            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}");
            app.Run();
        }
    }
}
