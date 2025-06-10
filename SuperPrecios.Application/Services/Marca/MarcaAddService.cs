using SuperPrecios.Application.DTO.Marca;
using SuperPrecios.Application.IServices.Marca;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Exceptions;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarcaCore = SuperPrecios.Domain.Entities.Marca;

namespace SuperPrecios.Application.Services.Marca
{
    public class MarcaAddService : IMarcaAddService
    {
        private readonly IMarcaRepository _marcaRepository;

        public MarcaAddService(IMarcaRepository marcaRepository)
        {
            _marcaRepository = marcaRepository;
        }
        public async Task AddAsync(DtoMarcaAdd dto)
        {
            if(dto == null) throw new ArgumentNullException("La marca no puede ser nula");
            MarcaCore marca = MapperMarca.ToMarca(dto);
            MarcaCore marcaBuscada = await _marcaRepository.GetByNombreAsync(marca);
            if (marcaBuscada != null) throw new MarcaException("La marca que se desea agregar ya existe");
            await _marcaRepository.AddAsync(marca);
        }
    }
}
