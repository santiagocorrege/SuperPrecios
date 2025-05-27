using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Usuario
{
    public class DtoUsuarioLogin
    {
        [Required(ErrorMessage = "Por favor ingrese un email")]
        public string Email { get; set; }
        
        public string Rol { get; set; }
    }
}
