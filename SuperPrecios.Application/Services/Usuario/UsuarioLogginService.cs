using SuperPrecios.Application.DTO.Usuario;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Application.IServices.Usuario;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Exceptions;
using UsuarioCore = SuperPrecios.Domain.Entities.Usuario;

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
            if (String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password)) throw new UsuarioException("El email y/o contrasena no pueden ser vacios");
            UsuarioCore usuario = await _repo.GetByUsuarioLogin(email, password);
             if (usuario == null) throw new UsuarioException("El usuario y/o contrasena no es valido");
            return MapperUsuario.ToDto(usuario);
        }    
    }
}
