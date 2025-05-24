using SuperPrecios.Application.DTO.Usuario;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Usuario;
using SuperPrecios.Application.Mappers;
using SuperPrecios.AuthenticationCore.Exceptions.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsuarioCore = SuperPrecios.AuthenticationCore.Entities.Usuario;

namespace SuperPrecios.Application.Services.Usuario
{
    public class UsuarioLogginService : IUsuarioLoginService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioLogginService(IUsuarioRepository repo)
        {
            _repo = repo;
        }
        public async Task<DtoUsuarioLogin> Run(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) throw new UsuarioException("El email y/o contrasena no pueden ser vacios");
            UsuarioCore usuario = await _repo.GetByUsuarioLogin(email, password);
            if (usuario == null) throw new UsuarioException("El usuario y/o contrasena no es valido");
            return MapperUsuario.ToDto(usuario);
        }    
    }
}
