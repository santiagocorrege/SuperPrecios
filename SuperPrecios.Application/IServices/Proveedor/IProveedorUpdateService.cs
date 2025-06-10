using SuperPrecios.Application.DTO.Miembro;
using SuperPrecios.Application.DTO.Proveedor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Proveedor
{
    public interface IProveedorUpdateService
    {
        public Task Run(DtoProveedorUpdate dto);
    }
}
