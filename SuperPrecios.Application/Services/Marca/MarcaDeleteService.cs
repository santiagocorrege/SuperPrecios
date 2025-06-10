using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Domain.Exceptions;
using SuperPrecios.Domain.IRepositories;
using MarcaCore = SuperPrecios.Domain.Entities.Marca;

namespace SuperPrecios.Application.Services.Marca
{
    public class MarcaDeleteService : IMarcaDeleteService
    {
        private readonly IMarcaRepository _marcaRepository;
        private readonly IProductoRepository _productoRepository;

        public MarcaDeleteService(IMarcaRepository marcaRepository, IProductoRepository productoRepository)
        {
            _marcaRepository = marcaRepository;
            _productoRepository = productoRepository;
        }

        public async Task Delete(int id)
        {
            //TODO: Podria agregar las validaciones de si existen instancias de esa marca evitarlo
            if(id < 1) throw new ArgumentOutOfRangeException("El id de la marca debe ser mayor a 0");
            MarcaCore marca = await _marcaRepository.GetByIdAsync(id);
            if (marca == null) throw new MarcaException("La marca que se desea eliminar no existe");
            var productosMarcaBuscados = await _productoRepository.GetProductosByMarca(marca);
            if (productosMarcaBuscados.Any()) throw new MarcaException("La marca no puede ser eliminada porque tiene productos asociados.");            
            await _marcaRepository.DeleteAsync(marca);
        }
    }
}
