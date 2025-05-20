using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Miembro
{
    public class DtoMiembroGet
    {
        [Required]
        public int Id { get; set; }
        [Required]

        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; init; }

    }
}
