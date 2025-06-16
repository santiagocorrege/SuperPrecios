using System.ComponentModel.DataAnnotations;

namespace SuperPrecios.Web.Models.Usuario
{
    public class VMUsuarioLogin
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]   
        public string Password { get; set; }

        [Display(Name = "Recuérdame")]
        public bool RememberMe { get; set; }
    }
}
