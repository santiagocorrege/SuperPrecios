using SuperPrecios.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.IServices.Miembro;
using SuperPrecios.Application.Services.Miembro;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Usuario;
using SuperPrecios.Application.Services.Usuario;

namespace SuperPrecios.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<SuperPreciosDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionPapeleria")));

            builder.Services.AddSession();

            //IoC : DI
            //DI: Repositories
            builder.Services.AddScoped<IMiembroRepository, MiembroRepositoryEF>();
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryEF>();

            //DI: Services
            //Usuario
            builder.Services.AddScoped<IUsuarioLoginService, UsuarioLogginService>();
            //Miembro
            builder.Services.AddScoped<IMiembroGet, MiembroGetService>();
            builder.Services.AddScoped<IMiembroAddService, MiembroAddService>();
            builder.Services.AddScoped<IMiembroUpdateService, MiembroUpdateService>();
            builder.Services.AddScoped<IMiembroDeleteService, MiembroDeleteService>();
                        
                        
            //Inversion
            var app = builder.Build();

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
