using System.ComponentModel.DataAnnotations;

namespace SuperPrecios.Web.Models.Usuario
{
    public class UsuarioLoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]   
        public string Password { get; set; }
    }
}
