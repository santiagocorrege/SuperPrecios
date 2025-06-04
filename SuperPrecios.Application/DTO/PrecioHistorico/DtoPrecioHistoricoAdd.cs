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
    public class DtoPrecioHistoricoAdd
    {
        [Length(2, 50, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
        [Required]
        public string Producto { get; set; }

        [Length(2, 50, ErrorMessage = "La marca debe tener entre 1 y 50 caracteres.")]
        [Required]
        public string Marca { get; set; }

        [Length(2, 50, ErrorMessage = "La categoria debe tener entre 1 y 50 caracteres.")]
        [Required]
        public string Categoria { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El id del supermercado no es valido.")]
        [Required]
        public int SupermercadoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [Required]
        public decimal Precio { get; set; }
    }
}
