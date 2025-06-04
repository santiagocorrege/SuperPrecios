using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.IServices.PrecioHistorico;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrecioHistoricoCore = SuperPrecios.Domain.Entities.PrecioHistorico;

namespace SuperPrecios.Application.Services.PrecioHistorico
{
    public class PrecioHistoricoAddService : IPrecioHistoricoAddService
    {

        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMarcaRepository _marcaRepository;
        private readonly IPrecioHistoricoRepository _precioHistoricoRepository;
        private readonly ISupermercadoRepository _supermercadoRepository;

        public PrecioHistoricoAddService(IProductoRepository productoRepository, ICategoriaRepository categoriaRepository, IMarcaRepository marcaRepository, IPrecioHistoricoRepository precioHistoricoRepository, ISupermercadoRepository supermercadoRepository)
        {
            _productoRepository = productoRepository;
            _categoriaRepository = categoriaRepository;
            _marcaRepository = marcaRepository;
            _precioHistoricoRepository = precioHistoricoRepository;
            _supermercadoRepository = supermercadoRepository;
        }

        public async Task AddAsync(List<DtoPrecioHistoricoAdd> dtoList)
        {
            if(dtoList == null || dtoList.Count == 0) throw new ArgumentException("La lista de precios historicos no puede ser nula o vacía.", nameof(dtoList));            
            IEnumerable<PrecioHistoricoCore> preciosHistorico = MapperPrecioHistorico.ToPrecioHistoricoList(dtoList);
            foreach(PrecioHistoricoCore precioHistorico in preciosHistorico)
            {
                if (precioHistorico == null) throw new ArgumentNullException("El precio historico no puede ser nulo");
                if (precioHistorico.Precio <= 0) throw new ArgumentException("El precio no puede ser menor a 0");
                if (precioHistorico.SupermercadoId <= 0) throw new ArgumentException("El id del supermercado no es valido");
                await _precioHistoricoRepository.AddAsync(precioHistorico);
            }         
        }

    }
}
