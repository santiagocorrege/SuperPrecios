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
            if (dto == null)
                throw new ArgumentNullException("La marca no puede ser nula");

            // ✅ CAMBIO: Validar que el DTO incluya el ID de Python
            if (dto.Id <= 0)
                throw new ArgumentException("El ID de la marca debe ser proporcionado por el sistema externo");

            MarcaCore marca = MapperMarca.ToMarca(dto);
            marca.Id = dto.Id; // ✅ NUEVO: Asignar ID de Python

            // ✅ CAMBIO: Buscar por ID en lugar de nombre
            MarcaCore marcaBuscada = await _marcaRepository.GetByIdAsync(marca.Id);
            if (marcaBuscada != null)
                throw new MarcaException($"La marca con ID {marca.Id} ya existe");

            await _marcaRepository.AddAsync(marca);
        }
    }
}
