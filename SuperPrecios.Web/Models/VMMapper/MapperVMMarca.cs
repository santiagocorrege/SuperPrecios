using Microsoft.AspNetCore.Mvc.Rendering;
using SuperPrecios.Application.DTO.Marca;

namespace SuperPrecios.Web.Models.VMMaper
{
    public class MapperVMMarca
    {
        public static SelectListItem ToSelectItem(DtoMarcaGet dto)
        {
            return new SelectListItem
            {
                Value = dto.Id.ToString(),
                Text = dto.Nombre
            };
        }

        public static IEnumerable<SelectListItem> ToSelectItem(IEnumerable<DtoMarcaGet> dtos)
        {
            return dtos.Select(dto => ToSelectItem(dto));
        }
    }
}
