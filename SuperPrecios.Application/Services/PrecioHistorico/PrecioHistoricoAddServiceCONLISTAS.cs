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

//namespace SuperPrecios.Application.Services.PrecioHistorico
//{
//    public class PrecioHistoricoAddServiceCONLISTAS : IPrecioHistoricoAddService
//    {
//        private readonly IProductoRepository _productoRepository;
//        private readonly ICategoriaRepository _categoriaRepository;
//        private readonly IMarcaRepository _marcaRepository;
//        private readonly IPrecioHistoricoRepository _precioHistoricoRepository;

//        public PrecioHistoricoAddServiceCONLISTAS(
//            IProductoRepository productoRepository,
//            ICategoriaRepository categoriaRepository,
//            IMarcaRepository marcaRepository,
//            IPrecioHistoricoRepository precioHistoricoRepository)
//        {
//            _productoRepository = productoRepository;
//            _categoriaRepository = categoriaRepository;
//            _marcaRepository = marcaRepository;
//            _precioHistoricoRepository = precioHistoricoRepository;
//        }

//        public async Task AddAsync(List<DtoPrecioHistoricoAdd> dtoList)
//        {
//            if (dtoList == null || dtoList.Count == 0)
//                throw new ArgumentException("La lista de precios históricos no puede ser nula o vacía.", nameof(dtoList));

//            try
//            {
//                var preciosHistorico = MapperPrecioHistorico.ToPrecioHistoricoList(dtoList);
//                var categoriasNombres = preciosHistorico.Select(p => p.Producto.Categoria.Producto).Distinct();
//                var marcasNombres = preciosHistorico.Select(p => p.Producto.Marca.Producto).Distinct();
//                var productosClave = preciosHistorico.Select(p => p.Producto.Producto).Distinct();

//                var categoriasExistentes = (await _categoriaRepository.GetByNombresAsync(categoriasNombres))
//                    .ToDictionary(c => c.Producto);
//                var marcasExistentes = (await _marcaRepository.GetByNombresAsync(marcasNombres))
//                    .ToDictionary(m => m.Producto);
//                var productosExistentes = (await _productoRepository.GetByNombresAsync(productosClave))
//                    .ToDictionary(p => p.Producto);

//                foreach (var precioHistorico in preciosHistorico)
//                {
//                    if (precioHistorico == null)
//                        throw new ArgumentNullException("Un elemento de la lista es nulo");

//                    if (precioHistorico.Precio <= 0)
//                        throw new ArgumentException("El precio no puede ser menor o igual a 0");

//                    if (precioHistorico.SupermercadoId <= 0)
//                        throw new ArgumentException("El ID del supermercado no es válido");

//                    // Resolver categoría
//                    var categoriaNombre = precioHistorico.Producto.Categoria.Producto;
//                    if (!categoriasExistentes.TryGetValue(categoriaNombre, out var categoria))
//                    {
//                        categoria = new Categoria(categoriaNombre);
//                        await _categoriaRepository.AddAsync(categoria);
//                        categoriasExistentes[categoriaNombre] = categoria;
//                    }
//                    precioHistorico.Producto.Categoria = categoria;

//                    // Resolver marca
//                    var marcaNombre = precioHistorico.Producto.Marca.Producto;
//                    if (!marcasExistentes.TryGetValue(marcaNombre, out var marca))
//                    {
//                        marca = new Marca(marcaNombre);
//                        await _marcaRepository.AddAsync(marca);
//                        marcasExistentes[marcaNombre] = marca;
//                    }
//                    precioHistorico.Producto.Marca = marca;

//                    // Resolver producto
//                    var productoNombre = precioHistorico.Producto.Producto;
//                    if (!productosExistentes.TryGetValue(productoNombre, out var producto))
//                    {
//                        producto = new Producto(productoNombre, marca, categoria);
//                        await _productoRepository.AddAsync(producto);
//                        productosExistentes[productoNombre] = producto;
//                    }
//                    precioHistorico.Producto = producto;

//                    // Agregar el precio histórico
//                    await _precioHistoricoRepository.AddAsync(precioHistorico);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error en el servicio de agregar precio histórico.", ex);
//            }
//        }
//    }
//}
