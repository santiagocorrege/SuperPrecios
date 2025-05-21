using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IRepository
{
    public interface IRepository<T> where T : class
    {
        public Task<T> GetByIdAsync(int id);

        public Task<IEnumerable<T>> GetAll();

        public Task AddAsync(T entity);

        public Task UpdateAsync(T entity);

        public Task DeleteAsync(int id);

    }
}
