using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperPrecios.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRutaService _categoriaRutaService;

        public CategoriaController(ICategoriaRutaService categoriaRutaService)
        {
            _categoriaRutaService = categoriaRutaService;
        }

        // GET: api/Categoria
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Llamas al método que reconstruye rutas
                IEnumerable<DtoCategoriaConRuta> categoriasConRuta =
                    await _categoriaRutaService.ObtenerCategoriasConRuta();

                return Ok(categoriasConRuta);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
