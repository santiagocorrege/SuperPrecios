using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SuperPrecios.Web.Models.Producto
{
    public class VMProductoAdd
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe poseer entre 2 y 50 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una Marca.")]
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una Categoría.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }
        
        public IEnumerable<SelectListItem> Marcas { get; set; }
        public IEnumerable<SelectListItem> Categorias { get; set; }
    }
}
