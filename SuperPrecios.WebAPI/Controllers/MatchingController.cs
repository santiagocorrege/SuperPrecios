using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.MiniPSS;
using SuperPrecios.Application.IServices.Matcher;

namespace SuperPrecios.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchingController : ControllerBase
    {
        private readonly IMatchingProcessService _matchingProcessService;

        public MatchingController(IMatchingProcessService matchingProcessService)
        {
            _matchingProcessService = matchingProcessService;
        }

        [HttpPost("process-matching-results")]
        public async Task<IActionResult> ProcessMatchingResults([FromBody] MiniPssResultadoDto dto)
        {
            // 1. Validaciones de entrada
            if (dto == null)
                return BadRequest("El cuerpo de la petición no puede ser nulo.");
            if (dto.Resultados == null || !dto.Resultados.Any())
                return BadRequest("Debe indicar la lista de resultados del matching.");
            if (dto.TotalProductos <= 0)
                return BadRequest("El total de productos debe ser mayor a 0.");

            try
            {                
                await _matchingProcessService.ProcessMatchingResultsAsync(dto);                
                return Ok($"Resultados de matching procesados correctamente. Total: {dto.TotalProductos}, Matched: {dto.ProductosMatched}, Nuevos: {dto.ProductosNuevos}");
            }
            catch (ArgumentException ex)
            {                
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {                
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Error interno del servidor: {ex.Message}"
                );
            }
        }
    }
}