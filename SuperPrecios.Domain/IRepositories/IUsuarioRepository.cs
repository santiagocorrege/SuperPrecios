using SuperPrecios.Domain.Entities;


namespace SuperPrecios.Application.IRepository
{
    public interface IUsuarioRepository
    {
        public Task<Usuario> GetByUsuarioLogin(string email, string password);
    }
}
