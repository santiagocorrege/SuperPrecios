using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.IServices.Categoria;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperPrecios.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaGetService _categoriaGetService;

        public CategoriaController(ICategoriaGetService categoriaGetService)
        {
            _categoriaGetService = categoriaGetService;
        }

        // GET: api/<CategoriasController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var categorias = await _categoriaGetService.GetAll();
                return Ok(categorias);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }           
        }
    
    }
}
