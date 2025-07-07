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
    //Utilizado para PrecioHistoricoAdd : By supermercado, viaja en un DTO junto con CategoriaId y SupermercadoId
    public class DtoPrecioHistoricoAddBySupermercado
    {
        [Length(2, 100, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Required]
        public string Nombre { get; set; }

        [Length(2, 50, ErrorMessage = "La marca debe tener entre 1 y 50 caracteres.")]
        [Required]
        public string Marca { get; set; }       

        [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Required]
        public decimal Precio { get; set; }

        [Length(2, 200, ErrorMessage = "La imagen debe tener entre 1 y 200 caracteres.")]
        public string? ImgUrl { get; set; }

        public string? Divisa { get; set; }
    }
}
