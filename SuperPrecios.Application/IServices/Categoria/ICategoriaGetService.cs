using SuperPrecios.Application.DTO.Categoria;
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
    }
}
