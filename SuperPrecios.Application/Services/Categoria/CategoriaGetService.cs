using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Categoria
{
    public class CategoriaGetService : ICategoriaGetService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaGetService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }
        public async Task<IEnumerable<DtoCategoriaGet>> GetAll()
        {
            var categoriasBuscadas = await _categoriaRepository.GetAllAsync();
            return MapperCategoria.ToDto(categoriasBuscadas);
        }

        public async Task<DtoCategoriaGet> GetById(int id)
        {
            if(id < 0) throw new CategoriaException ("El id de la categoría no es valido");
            var categoriaBuscada = await _categoriaRepository.GetByIdAsync(id);
            if (categoriaBuscada == null) throw new CategoriaException("La categoría que se desea buscar no existe");
            return MapperCategoria.ToDto(categoriaBuscada);
        }
    }
}
