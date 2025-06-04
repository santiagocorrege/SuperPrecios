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
        public async Task<IActionResult> Post([FromBody] List<DtoPrecioHistoricoAdd> dtoList)
        {
            if (dtoList == null || dtoList.Count <= 0)
            {
                return BadRequest("Debe indicar el precio historico a agregar");
            }
            try
            {
                await _precioHistoricoAddService.AddAsync(dtoList);
                return Ok("Precio historico agregado correctamente");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
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
                IEnumerable<DtoProductoPreciosHistoricos> productosConPreciosHistoricos = await _precioHistoricoGetService.GetAllBySupermercado(supermercadoId);
                return Ok(productosConPreciosHistoricos);
            }
            catch(Exception e)
            {
                return StatusCode(500, $"Error : {e.Message}");
            }            
        }

        /*
        // GET api/<PrecioHistoricoController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }        

        // PUT api/<PrecioHistoricoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<PrecioHistoricoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        */
    }
}
