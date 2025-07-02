using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Domain.Excepciones;
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
            if (categoriaBuscada != null) throw new ArgumentException("La categoría ya existe");
            if (dto.PadreId.HasValue)
            {
                categoria.PadreId = dto.PadreId.Value;
                var padre = await _categoriaRepository.GetByIdAsync(categoria.PadreId.Value);                    
                if (padre == null) throw new CategoriaException("No se encontro categoria con ese id para asignarla como padre");
                categoria.Padre = padre;
            }            
            await _categoriaRepository.AddAsync(categoria);
        }
    }
}
