using SuperPrecios.AuthenticationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IRepository
{
    public interface IAdministradorRepository : IRepository<Administrador>
    {
        public Task<Miembro> GetByEmailAsync(string email);
    }
}
