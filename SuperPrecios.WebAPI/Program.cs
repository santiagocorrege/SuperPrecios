
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
        public static void Main(string[] args)
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
                options.UseSqlServer(connectionString);
            });


            //IoC : DI
            //************DI: Repositories************
            builder.Services.AddScoped<IProductoRepository, ProductoRepositoryEF>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepositoryEF>();
            builder.Services.AddScoped<IMarcaRepository, MarcaRepositoryEF>();
            builder.Services.AddScoped<ISupermercadoRepository, SupermercadoRepositoryEF>();
            builder.Services.AddScoped<IPrecioHistoricoRepository, PrecioHistoricoRepositoryEF>();

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
    }
}
