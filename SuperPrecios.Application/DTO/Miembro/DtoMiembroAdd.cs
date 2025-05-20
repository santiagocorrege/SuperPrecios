using SuperPrecios.AutenticacionCore.ValueObject;
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
        [Required]
        public string Id { get; set; }
        [Required]        
        
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Email { get; init; }

        [Required]
        public string Password { get; set; }
    }
}
