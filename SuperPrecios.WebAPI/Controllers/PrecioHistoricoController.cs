using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.PrecioHistorico;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperPrecios.WebAPI.Controllers
{
    //[ApiConventionType(typeof(DefaultApiConventions))]
    [Route("api/[controller]")]
    [ApiController]
    public class PrecioHistoricoController : ControllerBase
    {
        private readonly IPrecioHistoricoAddService _precioHistoricoAddService;
        private readonly IPrecioHistoricoGetService _precioHistoricoGetService;

        public PrecioHistoricoController(IPrecioHistoricoAddService precioHistoricoAddService, IPrecioHistoricoGetService precioHistoricoGetService)
        {
            _precioHistoricoAddService = precioHistoricoAddService;
            _precioHistoricoGetService = precioHistoricoGetService;
        }

        // POST api/<PrecioHistoricoController>
        [HttpPost]
        public async Task<IActionResult> AddListBySupermercado(
            [FromBody] DtoPrecioHistoricoAddBySupermercadoList dto)
        {
            // 1. Validaciones de entrada
            if (dto == null)
                return BadRequest("El cuerpo de la petición no puede ser nulo.");

            if (dto.PreciosHistoricos == null || !dto.PreciosHistoricos.Any())
                return BadRequest("Debe indicar la lista de precios históricos a agregar.");

            if (dto.SupermercadoId <= 0 || dto.CategoriaId <= 0)
                return BadRequest("Los Id de supermercado y/o categoría no son válidos.");

            try
            {
                // 2. Llamada al servicio que orquesta el mapeo y la persistencia
                await _precioHistoricoAddService.AddBySupermercadoAsync(dto);

                // 3. Respuesta satisfactoria
                return Ok("Precios históricos agregados correctamente.");
            }
            catch (ArgumentException ex)
            {
                // Errores de validación desde el servicio o mapeo
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Cualquier otro error inesperado
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"Error interno del servidor: {ex.Message}"
                );
            }
        }


        // GET: api/<PrecioHistoricoController>
        [HttpGet("GetBySupermercado/{supermercadoId:int}")]
        public async Task<IActionResult> GetAllBySupermercado(int supermercadoId)
        {
            if(supermercadoId <= 0)
            {
                return BadRequest("El supermercado id no es un id valido");
            }
            try
            {
                IEnumerable<DtoProductoPreciosHistoricosXSupermercado> productosConPreciosHistoricos = await _precioHistoricoGetService.GetAllBySupermercado(supermercadoId);
                return Ok(productosConPreciosHistoricos);
            }
            catch(Exception e)
            {
                return StatusCode(500, $"Error : {e.Message}");
            }            
        }
    }
}
