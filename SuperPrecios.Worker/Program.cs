using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperPrecios.Worker;

namespace SuperPrecios.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Registrar HttpClient con configuración desde appsettings.json
                    services.AddHttpClient("Scraper", client =>
                    {
                        var baseUrl = hostContext.Configuration["Scraper:BaseUrl"]
                                      ?? throw new InvalidOperationException("Scraper:BaseUrl not configured");
                        client.BaseAddress = new Uri(baseUrl);
                        // Ajusta timeouts, headers, etc., si es necesario
                    })
                    // Opcional: controlar lifetime de handlers
                    // .SetHandlerLifetime(TimeSpan.Zero)
                    ;
                    services.AddHttpClient("SuperPreciosAPI", client =>
                    {
                        var baseUrl = hostContext.Configuration["SuperPreciosAPI:BaseUrl"]
                                      ?? throw new InvalidOperationException("SuperPreciosAPI:BaseUrl not configured");
                        client.BaseAddress = new Uri(baseUrl);
                        // Ajusta timeouts, headers, etc., si es necesario
                    });
                    // Registrar el Worker como servicio hospedado
                    services.AddHostedService<Worker>();
                });

            await builder.RunConsoleAsync();
        }
    }
}