using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.DTO.Marca;
using SuperPrecios.Application.DTO.PrecioHistorico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Producto
{
    public class DtoProductoGet
    {        
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DtoMarcaGet Marca { get; set; }
        public DtoCategoriaGet Categoria { get; set; }        

        public decimal Precio { get; set; }
        
        public IEnumerable<DtoPrecioHistoricoWOProducto> PreciosHistoricos { get; set; }
    }
}
