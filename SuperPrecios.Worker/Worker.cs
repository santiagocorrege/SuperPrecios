using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SuperPrecios.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        //Ventajas y desventajas con IHTTPClientFactory, new HttpClient();
        private readonly HttpClient _httpClientScraper;
        private readonly HttpClient _httpClientMatcher;
        private readonly HttpClient _httpClientSuperpreciosAPI;
        private readonly TimeSpan _delay = TimeSpan.FromMinutes(5);

        public Worker(ILogger<Worker> logger, IHttpClientFactory httpFactory)
        {
            _logger = logger;
            _httpClientScraper = httpFactory.CreateClient("Scraper");
            _httpClientSuperpreciosAPI = httpFactory.CreateClient("SuperPreciosAPI");
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {

                    //var supermarketUrl = "https://www.devoto.com.uy";
                    //string routes = await SiteMaps(supermarketUrl, ct);                                                            
                }
                catch (TaskCanceledException)
                {
                    // Cancelación solicitada, salimos sin log de error
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error al llamar al servicio de scraping");
                }

                await Task.Delay(_delay, ct);
            }
        }

        private async Task<string> SiteMaps(string baseUrl, CancellationToken ct)
        {
            HttpClient cliente = _httpClientScraper;
            
            var url = $"/sitemaps?base_url={Uri.EscapeDataString(baseUrl)}";
            var response = await _httpClientScraper.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync(ct);
            _logger.LogInformation("Scrape result: {body}", body);
            return body;
        }
        
    }
}