using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Producto
{
    public interface IProductoDeleteService
    {
        public Task Run(int idProducto);
    }
}
