using SuperPrecios.Application.DTO.Usuario;
using SuperPrecios.AutenticacionCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Mappers
{
    public class MapperUsuario
    {
        public static DtoUsuarioLogin ToDto(Usuario usuario)
        {
            return new DtoUsuarioLogin()
            {
                Email = usuario.Email.Valor,
                Rol = usuario.Rol()
            };            
        }
    }
}
