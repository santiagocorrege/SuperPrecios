using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Domain.IRepositories;
using CategoriaCore = SuperPrecios.Domain.Entities.Categoria;
namespace SuperPrecios.Application.Services.Categoria
{    
    public class CategoriaAddService : ICategoriaAddService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        public CategoriaAddService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository ?? throw new ArgumentNullException(nameof(categoriaRepository), "El repositorio de categorías no puede ser nulo");
        }
        public async Task Run(DtoCategoriaAdd dto) 
        {
            if (dto == null) throw new ArgumentNullException("La categoria no puede ser nula");
            CategoriaCore categoria = new CategoriaCore(dto.Nombre);
            var categoriaBuscada = await _categoriaRepository.GetByNombreAsync(categoria);
            if(categoriaBuscada != null) throw new ArgumentException("La categoría ya existe");
            await _categoriaRepository.AddAsync(categoria);
        }
    }
}
