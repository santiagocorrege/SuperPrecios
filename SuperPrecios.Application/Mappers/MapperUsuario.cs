using SuperPrecios.Application.DTO.Usuario;
using SuperPrecios.Domain.Entities;


namespace SuperPrecios.Application.Mappers
{
    public class MapperUsuario
    {
        public static DtoUsuarioLogin ToDto(Usuario usuario)
        {
            return new DtoUsuarioLogin()
            {
                Id = usuario.Id,
                Email = usuario.Email.Valor,
                Rol = usuario.Rol()
            };            
        }
    }
}
