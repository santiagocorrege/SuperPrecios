using SuperPrecios.Application.IRepository;
using SuperPrecios.AuthenticationCore.Entities;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.IRepositories
{
    public interface IProveedorRepository : IRepository<Proveedor>
    {
        public Task<Proveedor> GetByEmailAsync(string stringEmail);
        
    }
}
