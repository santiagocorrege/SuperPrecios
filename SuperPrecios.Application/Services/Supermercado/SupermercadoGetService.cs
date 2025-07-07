using SuperPrecios.Application.DTO.Supermercado;
using SuperPrecios.Application.IServices.Supermercado;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Supermercado
{
    public class SupermercadoGetService : ISupermercadoGetService
    {
        private readonly ISupermercadoRepository _repository;
            
        public SupermercadoGetService(ISupermercadoRepository repo)
        {
            _repository = repo;
        }
        public async Task<IEnumerable<DtoSupermercadoGet>> GetAllWAddress()
        {
            var Supermercados = await _repository.GetAllAsync();
            var SupermercadosWAdress = Supermercados.Where(s => s.WebsiteUrl != null).ToList();
            return MapperSupermercado.ToDto(SupermercadosWAdress);
        }
    }
}
