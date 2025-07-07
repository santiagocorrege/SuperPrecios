using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.PrecioHistorico
{
    public interface IPrecioHistoricoAddService
    {        
        public Task AddAsync(List<DtoPrecioHistoricoAdd> dtoList);

        public Task AddBySupermercadoAsync(DtoPrecioHistoricoAddBySupermercadoList dtoList);
    }
}
