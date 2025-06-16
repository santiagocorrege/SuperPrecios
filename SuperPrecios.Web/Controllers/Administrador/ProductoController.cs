using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Web.Models.Producto;
using SuperPrecios.Web.Models.VMMaper;
using SuperPrecios.Web.Models.VMMapper;

namespace SuperPrecios.Web.Controllers.Administrador
{    
    public class ProductoController : Controller
    {        
        private readonly IProductoAddService _productoAddService;
        private readonly IProductoGetService _productoGetService;
        private readonly IProductoDeleteService _productoDeleteService;
        private readonly ICategoriaGetService _categoriaGetService;
        private readonly IMarcaGetService _marcaGetService;

        public ProductoController(IProductoAddService productoAddService, IProductoGetService productoGetService, IProductoDeleteService productoDeleteService, ICategoriaGetService categoriaGetService, IMarcaGetService marcaGetService)
        {
            _productoAddService = productoAddService;
            _productoGetService = productoGetService;
            _productoDeleteService = productoDeleteService;
            _categoriaGetService = categoriaGetService;
            _marcaGetService = marcaGetService;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var productos = await _productoGetService.GetAllAsync();
                if (productos == null || productos.Count() == 0) throw new Exception("No existen productos");              
                return View(productos);            
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // GET: ProductosController/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var dtoProducto = await _productoGetService.GetByIdAsync(id);
                return View(dtoProducto);
            }
            catch (Exception e) 
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }            
        }

        // GET: ProductosController/Create
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var marcas = await _marcaGetService.GetAll();
                if (marcas == null || marcas.Count() == 0) throw new Exception("No existen marcas registradas, para agregar un producto registra una marca primero");
                var categorias = await _categoriaGetService.GetAll();
                if (categorias == null || categorias.Count() == 0) throw new Exception("No existen categorias registradas, para agregar un producto registra una categoria primero");
                VMProductoAdd viewModel = new VMProductoAdd
                {
                    Marcas = MapperVMMarca.ToSelectItem(marcas),
                    Categorias = MapperVMCategoria.ToSelectItem(categorias)
                };
                return View(viewModel);
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }                        
        }

        // POST: ProductosController/Create
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VMProductoAdd vm)
        {
            try
            {
                if(vm == null) throw new ArgumentNullException("El producto no puede ser nulo");   
                DtoProductoAdd dto = new DtoProductoAdd
                {
                    Nombre = vm.Nombre,
                    MarcaId = vm.MarcaId,
                    CategoriaId = vm.CategoriaId
                };
                await _productoAddService.AddAsync(dto);
                TempData["Message"] = "Producto creado correctamente"; //VIEWBAG SERIA
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message;                //VIEWBAG SERIA
            }
            //ViewModelProductoAdd viewModel = new ViewModelProductoAdd
            //{
            //    Marcas = vm.Marcas,
            //    Categorias = vm.Categorias
            //};
            //return View(vm);
            return RedirectToAction(nameof(Index));
        }

        // GET: ProductosController/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1) throw new ArgumentException("Id de producto invalido");
                var dtoProducto = await _productoGetService.GetByIdAsync(id);
                return View(dtoProducto);
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProductosController/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(DtoProductoGet dto)
        {
            try
            {
                if (dto.Id < 1) throw new ArgumentException("Id de producto invalido");
                await _productoDeleteService.Run(dto.Id);
                TempData["Message"] = "Producto eliminado correctamente";                
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;                
            }
            return RedirectToAction(nameof(Index));
        }


        /*
        // GET: ProductosController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if(id < 1) throw new ArgumentException("Id de producto invalido");
                var dtoProducto = await _productoGetService.GetByIdAsync(id);
                return View(dtoProducto);
            }
            catch(Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ProductosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        */
    }
}
