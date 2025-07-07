using SuperPrecios.Application.DTO.Supermercado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Supermercado
{
    public interface ISupermercadoGetService
    {
        public Task<IEnumerable<DtoSupermercadoGet>> GetAllWAddress();
    }
}
