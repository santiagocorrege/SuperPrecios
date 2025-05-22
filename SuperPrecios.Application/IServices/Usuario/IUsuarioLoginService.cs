using SuperPrecios.Application.DTO.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Usuario
{
    public interface IUsuarioLoginService
    {
        public Task<DtoUsuarioLogin> Run(string email, string password);
    }
}
