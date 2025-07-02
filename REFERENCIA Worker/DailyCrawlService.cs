// Install-Package Cronos
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class DailyCrawlService : BackgroundService
{
    private readonly ILogger<DailyCrawlService> _log;
    private readonly HttpClient _http;
    // every day at 02:00 local time
    private readonly CronExpression _cron = CronExpression.Parse("0 0 2 * * *");
    public DailyCrawlService(ILogger<DailyCrawlService> log, HttpClient http)
    {
        _log = log;
        _http = http;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var next = _cron.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                _log.LogInformation("Waiting {Delay} until next scrape", delay);
                await Task.Delay(delay, stoppingToken);

                try
                {
                    var resp = await _http.PostAsync("http://localhost:8000/run_scrape", null, stoppingToken);
                    resp.EnsureSuccessStatusCode();
                    _log.LogInformation("Scrape run succeeded");
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Scrape run failed");
                }
            }
        }
    }
}
