using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.DTO.Supermercado;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.PrecioHistorico
{
    //Utilizado para PrecioHistoricoAdd : By supermercado, viaja en un DTO junto con CategoriaId y SupermercadoId en DtoPrecioHistoricoAddBySupermercadoList
    public class DtoPrecioHistoricoAddBySupermercadoList
    {        
        [Range(0, int.MaxValue, ErrorMessage="El id del supermercado no es valido")]
        public int SupermercadoId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El id de la categoria no es valido")]
        public int CategoriaId { get; set; }

        //Annotation para count > 0 ?        
        public List<DtoPrecioHistoricoAddBySupermercado> PreciosHistoricos { get; set; } = new List<DtoPrecioHistoricoAddBySupermercado>();
    }
}
