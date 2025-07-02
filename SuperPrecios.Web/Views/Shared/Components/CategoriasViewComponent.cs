using Microsoft.AspNetCore.Mvc;
using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Domain.TAD;

namespace SuperPrecios.Web.Views.Shared.Component
{
    public class CategoriasViewComponent : ViewComponent
    {
        private readonly ICategoriaGetService _categoriaGetService;

        public CategoriasViewComponent(ICategoriaGetService categoriaService)
        {
            _categoriaGetService = categoriaService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Nodo<DtoCategoriaGet>> arbol = await _categoriaGetService.GetArbolesCategoria();
            return View(arbol);
        }
    }
}
