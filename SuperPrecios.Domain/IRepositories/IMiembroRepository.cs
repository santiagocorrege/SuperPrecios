using SuperPrecios.Domain.Entities;


namespace SuperPrecios.Application.IRepository
{
    public interface IMiembroRepository : IRepository<Miembro>  
    {
        public Task<Miembro> GetByEmailAsync(string email);

        public Task<IEnumerable<Miembro>> GetByEmailListAsync(string email);        

    }
}
