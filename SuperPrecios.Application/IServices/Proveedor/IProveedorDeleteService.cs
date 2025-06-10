using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Proveedor
{
    public interface IProveedorDeleteService
    {
        public Task Run(int id);
    }
}
