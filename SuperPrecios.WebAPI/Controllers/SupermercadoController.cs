using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.Supermercado;
using SuperPrecios.Application.IServices.Supermercado;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperPrecios.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupermercadoController : ControllerBase
    {
        private readonly ISupermercadoGetService _supermercadoGetService;
        public SupermercadoController(ISupermercadoGetService superGetService)
        {
            _supermercadoGetService = superGetService;
        }
        // GET: api/<SupermercadoController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _supermercadoGetService.GetAllWAddress());
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


    }
}
