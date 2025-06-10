using SuperPrecios.Application.DTO.Marca;
using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Marca
{
    public class MarcaGetService : IMarcaGetService
    {
        private readonly IMarcaRepository _marcaRepository;

        public MarcaGetService(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }

        public async Task<IEnumerable<DtoMarcaGet>> GetAll()
        {
            var marcasBuscadas = await _marcaRepository.GetAllAsync();
            return MapperMarca.ToDto(marcasBuscadas);
        }

        public async Task<DtoMarcaGet> GetById(int id)
        {
            if(id < 1) throw new ArgumentException("Id de marca invalido");
            var marcaBuscada = await _marcaRepository.GetByIdAsync(id);
            if (marcaBuscada == null) throw new KeyNotFoundException("La marca con el id especificado no existe");
            return MapperMarca.ToDto(marcaBuscada);
        }
    }
}
