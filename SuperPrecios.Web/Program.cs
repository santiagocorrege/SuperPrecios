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
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Application.Services.Producto;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Application.Services.Categoria;
using SuperPrecios.Application.Services.Marca;
using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Application.Services.Proveedor;
using SuperPrecios.Application.IServices.Proveedor;
using Microsoft.AspNetCore.Authentication.Cookies;
using SuperPrecios.Web.Extensions;

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
            builder.Services.AddScoped<IProveedorRepository, ProveedorRepositoryEF>();

            //DI: Services
            //Usuario
            builder.Services.AddScoped<IUsuarioLoginService, UsuarioLogginService>();
            //Miembro
            builder.Services.AddScoped<IMiembroGet, MiembroGetService>();
            builder.Services.AddScoped<IMiembroAddService, MiembroAddService>();
            builder.Services.AddScoped<IMiembroUpdateService, MiembroUpdateService>();
            builder.Services.AddScoped<IMiembroDeleteService, MiembroDeleteService>();
            //Proveedor
            builder.Services.AddScoped<IProveedorGetService, ProveedorGetService>();
            builder.Services.AddScoped<IProveedorAddService, ProveedorAddService>();
            builder.Services.AddScoped<IProveedorDeleteService, ProveedorDeleteService>();
            builder.Services.AddScoped<IProveedorUpdateService, ProveedorUpdateService>();
            //PrecioHistorico
            builder.Services.AddScoped<IPrecioHistoricoAddService, PrecioHistoricoAddService>();
            builder.Services.AddScoped<IPrecioHistoricoGetService, PrecioHistoricoGetService>();
            //Producto
            builder.Services.AddScoped<IProductoAddService, ProductoAddService>();
            builder.Services.AddScoped<IProductoGetService, ProductoGetService>();
            builder.Services.AddScoped<IProductoDeleteService, ProductoDeleteService>();
            //Categoria
            builder.Services.AddScoped<ICategoriaGetService, CategoriaGetService>();
            builder.Services.AddScoped<ICategoriaAddService, CategoriaAddService>();
            builder.Services.AddScoped<ICategoriaDeleteService, CategoriaDeleteService>();
            //Marca
            builder.Services.AddScoped<IMarcaGetService, MarcaGetService>();
            builder.Services.AddScoped<IMarcaAddService, MarcaAddService>();            
            builder.Services.AddScoped<IMarcaDeleteService, MarcaDeleteService>();

            //Cookies - Authentication Cookies cifradas - Se guarda en extensions de .Web
            builder.Services
                   .AddCookieAuthentication();

            //Inversion??
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

            //Added
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Login}/{id?}");
            app.Run();
        }
    }
}
