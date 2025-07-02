using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Domain.TAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Categoria
{
    public interface ICategoriaGetService
    {
        public Task<IEnumerable<DtoCategoriaGet>> GetAll();
        public Task<DtoCategoriaGet> GetById(int id);

        public Task<List<Nodo<DtoCategoriaGet>>> GetArbolesCategoria();

        Task<List<int>> GetDescendantCategoryIdsAsync(int categoriaId);

    }
}
