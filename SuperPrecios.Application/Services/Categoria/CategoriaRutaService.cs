using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Categoria
{
    public class CategoriaRutaService : ICategoriaRutaService
    {
        private readonly ICategoriaGetService _categoriaGetService;

        public CategoriaRutaService(ICategoriaGetService categoriaGetService)
        {
            _categoriaGetService = categoriaGetService;
        }

        public async Task<IEnumerable<DtoCategoriaConRuta>> ObtenerCategoriasConRuta()
        {
            var categorias = await _categoriaGetService.GetAll();               // trae List<DtoCategoriaGet>
            var dict = categorias.ToDictionary(c => c.Id);                // mapeo por Id
            var resultado = new List<DtoCategoriaConRuta>();

            foreach (var cat in categorias)
            {
                // reconstrucción de ruta padre→hijo
                var names = new List<string>();
                var actual = cat;
                while (actual != null)
                {
                    names.Insert(0, actual.Nombre);
                    actual = actual.PadreId.HasValue
                             ? dict[actual.PadreId.Value]
                             : null;
                }

                resultado.Add(new DtoCategoriaConRuta
                {
                    Id = cat.Id,
                    Nombre = cat.Nombre,
                    PadreId = cat.PadreId,
                    RutaCompleta = string.Join(" > ", names)
                });
            }

            return resultado;
        }
        }    
}
