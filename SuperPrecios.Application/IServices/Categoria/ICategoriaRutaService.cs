using SuperPrecios.Application.DTO.Categoria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Categoria
{
    public interface ICategoriaRutaService
    {
        /// <summary>
        /// Obtiene todas las categorías con su ruta completa (p. ej. "Alimentos > Bebidas > Gaseosas").
        /// </summary>
        public Task<IEnumerable<DtoCategoriaConRuta>> ObtenerCategoriasConRuta();
    }
}
