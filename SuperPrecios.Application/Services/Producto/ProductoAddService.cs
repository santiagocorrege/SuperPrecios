using SuperPrecios.Application.DTO.PrecioHistorico;
using SuperPrecios.Application.DTO.Producto;
using SuperPrecios.Application.IServices.Producto;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Producto
{
    public class ProductoAddService : IProductoAddService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMarcaRepository _marcaRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        public ProductoAddService(IProductoRepository productoRepo, IMarcaRepository marcaRepository, ICategoriaRepository categoriaRepository)
        {
            _productoRepository = productoRepo;
            _marcaRepository = marcaRepository;
            _categoriaRepository = categoriaRepository;
        }
        public Task AddAsync(DtoPrecioHistoricoAdd dto)
        {
            if (dto == null) throw new ArgumentNullException("Error: El precio producto a agregar no puede estar vacio");
            //return new Producto(dto.Nombre, marca, categoria);


        }

    }
}
