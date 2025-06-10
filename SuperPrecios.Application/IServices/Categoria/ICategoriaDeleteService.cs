using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Categoria
{
    public interface ICategoriaDeleteService
    {
        public Task Run(int idCategoria);
    }
}
