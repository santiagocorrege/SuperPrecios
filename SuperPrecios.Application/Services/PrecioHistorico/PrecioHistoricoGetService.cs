using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
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
using ProductoCore = SuperPrecios.Domain.Entities.Producto;
using SupermercadoCore = SuperPrecios.Domain.Entities.Supermercado;

namespace SuperPrecios.Application.Services.PrecioHistorico
{
    public class PrecioHistoricoGetService : IPrecioHistoricoGetService
    {
        private readonly IPrecioHistoricoRepository _precioHistoricoRepository;
        private readonly ISupermercadoRepository _SupermercadoRepository;

        //DI
        public PrecioHistoricoGetService(IPrecioHistoricoRepository repository, ISupermercadoRepository SupermercadoRepository)
        {
            _precioHistoricoRepository = repository;
            _SupermercadoRepository = SupermercadoRepository;
        }

        public async Task<IEnumerable<DtoProductoPreciosHistoricosXSupermercado>> GetAllBySupermercado(int SupermercadoId)
        {
            if (SupermercadoId <= 0)
            {
                throw new ArgumentException("Error: El id del Supermercado no es valido");
            }
            try
            {
                SupermercadoCore super = await _SupermercadoRepository.GetByIdAsync(SupermercadoId);
                if (super == null)
                {
                    throw new ArgumentException("Error: El Supermercado no existe");
                }
                IEnumerable<ProductoCore> productosConPreciosHistoricos  = await _precioHistoricoRepository.GetAllBySupermercado(SupermercadoId);
                return MapperProducto.ToDtoWPreciosHistoricos(productosConPreciosHistoricos, super.Nombre);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los productos con precios historicos del Supermercado: {ex.Message}", ex);
            }
            
        }

        public async Task<IEnumerable<DtoPrecioHistoricoWOProducto>> GetPrecioHistoricoProductoBySupermercado(int SupermercadoId, int productoId)
        {
            if (SupermercadoId <= 0 || productoId <= 0 )
            {
                throw new ArgumentException("Error: El id del Supermercado/producto no es valido");
            }
            try
            {
                SupermercadoCore super = await _SupermercadoRepository.GetByIdAsync(SupermercadoId);
                if (super == null)
                {
                    throw new ArgumentException("Error: El Supermercado no existe");
                }
                IEnumerable<PrecioHistoricoCore> preciosHistoricos = await _precioHistoricoRepository.GetPrecioHistoricoProductoBySupermercado(SupermercadoId, productoId);
                return MapperPrecioHistorico.ToDtoCompletoWoProducto(preciosHistoricos);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los productos con precios historicos del Supermercado: {ex.Message}", ex);
            }
        }

    }
}
