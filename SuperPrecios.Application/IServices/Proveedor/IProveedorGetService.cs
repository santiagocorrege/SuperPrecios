using SuperPrecios.Application.DTO.Proveedor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Proveedor
{
    public interface IProveedorGetService
    {
        public Task<IEnumerable<DtoProveedorGet>> Run();
        public Task<DtoProveedorGet> RunById(int id);
        public Task<DtoProveedorUpdate> RunByIdUpdate(int id);

    }
}
