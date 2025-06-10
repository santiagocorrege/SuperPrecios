using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.Services.Categoria
{
    public class CategoriaDeleteService : ICategoriaDeleteService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaDeleteService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }
        public async Task Run(int idCategoria)
        {
            if (idCategoria < 1) throw new ArgumentOutOfRangeException("El id de la categoria no es valido");
            var categoria = await _categoriaRepository.GetByIdAsync(idCategoria);
            if (categoria == null) throw new CategoriaException("La categoria que desea eliminar no existe");
            await _categoriaRepository.DeleteAsync(categoria);
        }
    }
}
