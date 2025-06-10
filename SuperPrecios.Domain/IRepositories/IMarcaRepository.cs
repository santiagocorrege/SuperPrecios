using SuperPrecios.Application.IRepository;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.IRepositories
{
    public interface IMarcaRepository : IRepository<Marca>
    {         
        public Task<Marca> GetByNombreAsync(Marca marca);
        
    }
}
