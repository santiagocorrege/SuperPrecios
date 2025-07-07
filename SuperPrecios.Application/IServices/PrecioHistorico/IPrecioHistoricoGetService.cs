using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.PrecioHistorico
{
    public interface IPrecioHistoricoGetService
    {
        public Task<IEnumerable<DtoProductoPreciosHistoricosXSupermercado>> GetAllBySupermercado(int SupermercadoId);

        public Task<IEnumerable<DtoPrecioHistoricoWOProducto>> GetPrecioHistoricoProductoBySupermercado(int SupermercadoId, int productoId);
        
    }
}
