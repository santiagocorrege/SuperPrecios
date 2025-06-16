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

namespace SuperPrecios.Application.Services.PrecioHistorico
{
    public class PrecioHistoricoGetService : IPrecioHistoricoGetService
    {
        private readonly IPrecioHistoricoRepository _precioHistoricoRepository;
        private readonly ISupermercadoRepository _supermercadoRepository;

        //DI
        public PrecioHistoricoGetService(IPrecioHistoricoRepository repository, ISupermercadoRepository supermercadoRepository)
        {
            _precioHistoricoRepository = repository;
            _supermercadoRepository = supermercadoRepository;
        }

        public async Task<IEnumerable<DtoProductoPreciosHistoricosXSupermercado>> GetAllBySupermercado(int supermercadoId)
        {
            if (supermercadoId <= 0)
            {
                throw new ArgumentException("Error: El id del supermercado no es valido");
            }
            try
            {
                Supermercado super = await _supermercadoRepository.GetByIdAsync(supermercadoId);
                if (super == null)
                {
                    throw new ArgumentException("Error: El supermercado no existe");
                }
                IEnumerable<ProductoCore> productosConPreciosHistoricos  = await _precioHistoricoRepository.GetAllBySupermercado(supermercadoId);
                return MapperProducto.ToDtoWPreciosHistoricos(productosConPreciosHistoricos, super.Nombre);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los productos con precios historicos del supermercado: {ex.Message}", ex);
            }
            
        }

        public async Task<IEnumerable<DtoPrecioHistoricoWOProducto>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, int productoId)
        {
            if (supermercadoId <= 0 || productoId <= 0 )
            {
                throw new ArgumentException("Error: El id del supermercado/producto no es valido");
            }
            try
            {
                Supermercado super = await _supermercadoRepository.GetByIdAsync(supermercadoId);
                if (super == null)
                {
                    throw new ArgumentException("Error: El supermercado no existe");
                }
                IEnumerable<PrecioHistoricoCore> preciosHistoricos = await _precioHistoricoRepository.GetPrecioHistoricoProductoBySupermercado(supermercadoId, productoId);
                return MapperPrecioHistorico.ToDtoCompletoWoProducto(preciosHistoricos);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los productos con precios historicos del supermercado: {ex.Message}", ex);
            }
        }

    }
}
