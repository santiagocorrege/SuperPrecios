using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Application.IServices.Producto;

namespace SuperPrecios.Web.Controllers
{
    public class ProductoMiembroController : Controller
    {        
        private readonly IProductoGetService _productoGetService;        
        private readonly ICategoriaGetService _categoriaGetService;
        private readonly IMarcaGetService _marcaGetService;

        public ProductoMiembroController(IProductoGetService productoGetService, ICategoriaGetService categoriaGetService, IMarcaGetService marcaGetService)
        {            
            _productoGetService = productoGetService;            
            _categoriaGetService = categoriaGetService;
            _marcaGetService = marcaGetService;
        }

        // GET: ProductoMiembroController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductoMiembroController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

    }
}
