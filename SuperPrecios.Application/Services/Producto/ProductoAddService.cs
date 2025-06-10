using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductoCore = SuperPrecios.Domain.Entities.Producto;

namespace SuperPrecios.Application.Services.Producto
{
    public class ProductoAddService : IProductoAddService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoAddService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task AddAsync(DtoProductoAdd dto)
        {
            if (dto == null) throw new ArgumentException("El producto que se desea agregar posee valores vacios");            
            ProductoCore producto = new ProductoCore(dto.Nombre, dto.MarcaId, dto.CategoriaId);
            await _productoRepository.AddAsync(producto);
        }
    }
}
