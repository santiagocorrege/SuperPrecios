using Microsoft.AspNetCore.Mvc.Rendering;
using SuperPrecios.Application.DTO.Categoria;

namespace SuperPrecios.Web.Models.VMMapper
{
    public class MapperVMCategoria
    {
        public static SelectListItem ToSelectItem(DtoCategoriaGet dto)
        {
            return new SelectListItem
            {
                Value = dto.Id.ToString(),
                Text = dto.Nombre
            };
        }

        public static IEnumerable<SelectListItem> ToSelectItem(IEnumerable<DtoCategoriaGet> dtos)
        {
            return dtos.Select(dto => ToSelectItem(dto));
        }
    }
}
