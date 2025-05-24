using SuperPrecios.AuthenticationCore.ValueObject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Miembro
{
    public class DtoMiembroAdd
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; init; }

        [Required(ErrorMessage = "La contrasena es obligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
