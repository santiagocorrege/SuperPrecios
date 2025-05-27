using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Categoria
{
    public class DtoCategoriaAdd
    {
        [Length(2, 50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        [Required]
        public string Nombre { get; set; }
    }
}
