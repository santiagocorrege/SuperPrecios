using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.MiniPSS;
using SuperPrecios.Application.IServices.Matcher;
using System.Diagnostics;

namespace SuperPrecios.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchingController : ControllerBase
    {
        private readonly IMatchingProcessService _matchingProcessService;
        private readonly ILogger<MatchingController> _logger;

        public MatchingController(
            IMatchingProcessService matchingProcessService,
            ILogger<MatchingController> logger)
        {
            _matchingProcessService = matchingProcessService;
            _logger = logger;
        }

        [HttpPost("process-matching-results")]
        public async Task<IActionResult> ProcessMatchingResults([FromBody] MiniPssResultadoDto dto)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8]; // ID corto para logs
            _logger.LogInformation(dto.ToString());
            _logger.LogInformation("[{RequestId}] Iniciando procesamiento de matching con {TotalProductos} productos",
                requestId, dto?.TotalProductos ?? 0);

            // ✅ VALIDACIONES DE ENTRADA MEJORADAS
            if (dto == null)
            {
                _logger.LogWarning("[{RequestId}] Request body nulo", requestId);
                return BadRequest("El cuerpo de la petición no puede ser nulo.");
            }

            if (dto.Resultados == null || !dto.Resultados.Any())
            {
                _logger.LogWarning("[{RequestId}] Lista de resultados vacía", requestId);
                return BadRequest("Debe indicar la lista de resultados del matching.");
            }

            if (dto.TotalProductos <= 0)
            {
                _logger.LogWarning("[{RequestId}] Total productos inválido: {TotalProductos}", requestId, dto.TotalProductos);
                return BadRequest("El total de productos debe ser mayor a 0.");
            }

            // ✅ VALIDACIÓN ADICIONAL: Verificar consistencia de datos y calcular estadísticas
            var totalProductosCalculado = dto.Resultados.Sum(r => r.ProductosProcesados?.Count ?? 0);

            // Calcular estadísticas de matching
            var productosMatched = dto.Resultados.Sum(r =>
                r.ProductosProcesados?.Count(p => p.Matched) ?? 0);
            var productosNuevos = dto.Resultados.Sum(r =>
                r.ProductosProcesados?.Count(p => !p.Matched) ?? 0);

            if (Math.Abs(totalProductosCalculado - dto.TotalProductos) > (dto.TotalProductos * 0.1)) // Tolerancia del 10%
            {
                _logger.LogWarning("[{RequestId}] Inconsistencia en total de productos. Declarado: {Declarado}, Calculado: {Calculado}",
                    requestId, dto.TotalProductos, totalProductosCalculado);
                return BadRequest($"Inconsistencia en el total de productos. Declarado: {dto.TotalProductos}, Calculado: {totalProductosCalculado}");
            }

            _logger.LogInformation("[{RequestId}] Estadísticas de matching - Total: {Total}, Matched: {Matched}, Nuevos: {Nuevos}",
                requestId, totalProductosCalculado, productosMatched, productosNuevos);

            try
            {
                _logger.LogInformation("[{RequestId}] Iniciando procesamiento optimizado para {SupermercadosCount} supermercados",
                    requestId, dto.Resultados.Count);

                // ✅ PROCESAMIENTO OPTIMIZADO (sin configurar HttpContext.RequestTimeout)
                await _matchingProcessService.ProcessMatchingResultsAsync(dto);

                stopwatch.Stop();

                var resultado = new
                {
                    RequestId = requestId,
                    Status = "Success",
                    TotalProductos = dto.TotalProductos,
                    ProductosMatched = productosMatched,
                    ProductosNuevos = productosNuevos,
                    SupermercadosProcesados = dto.Resultados.Count,
                    TiempoProcesamientoSegundos = stopwatch.Elapsed.TotalSeconds,
                    ThroughputProductosPorSegundo = Math.Round(dto.TotalProductos / stopwatch.Elapsed.TotalSeconds, 2),
                    EficienciaMatching = productosMatched > 0 ? Math.Round((double)productosMatched / dto.TotalProductos * 100, 1) : 0
                };

                _logger.LogInformation("[{RequestId}] Procesamiento exitoso en {TiempoSegundos:F2}s - Throughput: {Throughput} productos/seg - Matched: {Matched}% ({MatchedCount}/{Total})",
                    requestId, resultado.TiempoProcesamientoSegundos, resultado.ThroughputProductosPorSegundo,
                    resultado.EficienciaMatching, productosMatched, dto.TotalProductos);

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning("[{RequestId}] Error de validación en {TiempoSegundos:F2}s: {Error}",
                    requestId, stopwatch.Elapsed.TotalSeconds, ex.Message);

                return BadRequest(new
                {
                    RequestId = requestId,
                    Status = "ValidationError",
                    Error = ex.Message,
                    TiempoProcesamientoSegundos = stopwatch.Elapsed.TotalSeconds
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "[{RequestId}] Error interno en {TiempoSegundos:F2}s",
                    requestId, stopwatch.Elapsed.TotalSeconds);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        RequestId = requestId,
                        Status = "InternalError",
                        Error = $"Error interno del servidor: {ex.Message}",
                        TiempoProcesamientoSegundos = stopwatch.Elapsed.TotalSeconds,
                        Details = ex.InnerException?.Message // Solo para desarrollo
                    });
            }
        }

        // ✅ ENDPOINT: Verificar estado del sistema
        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "Optimized"
            });
        }

        // ✅ ENDPOINT: Obtener estadísticas de procesamiento
        [HttpGet("stats")]
        public IActionResult GetProcessingStats()
        {
            return Ok(new
            {
                Status = "Ready",
                OptimizationsActive = new[]
                {
                    "Bulk Inserts",
                    "Batch Queries",
                    "Transaction Scoped",
                    "Memory Optimized",
                    "Extended Timeouts"
                },
                RecommendedBatchSize = "< 50,000 productos por request",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}