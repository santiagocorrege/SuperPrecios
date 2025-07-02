using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.IServices.Categoria;
using SuperPrecios.Application.Mappers;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Domain.IRepositories;
using SuperPrecios.Domain.TAD;
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

        public async Task<List<Nodo<DtoCategoriaGet>>> GetArbolesCategoria()
        {
            var raices = new List<Nodo<DtoCategoriaGet>>();
            var categorias = await _categoriaRepository.GetAllAsync();
            var listaCategorias = MapperCategoria.ToDto(categorias);
            var dic = new Dictionary<int, Nodo<DtoCategoriaGet>>();

            // Crear los nodos y agregarlos al diccionario
            foreach (var dto in listaCategorias)
            {
                if (!dic.ContainsKey(dto.Id))
                {
                    dic[dto.Id] = new Nodo<DtoCategoriaGet>(dto, dto.Id);
                }
                else
                {
                    throw new InvalidOperationException($"Id duplicado detectado en categoría: {dto.Id} - {dto.Nombre}");
                }
            }
            // Enlazar los nodos padre-hijo
            foreach (var nodo in dic.Values)
            {
                if (nodo.Valor.PadreId.HasValue)
                {
                    if (dic.TryGetValue(nodo.Valor.PadreId.Value, out var nodoPadre))
                    {
                        nodoPadre.AgregarHijo(nodo);
                    }
                    else
                    {
                        throw new InvalidOperationException($"No se encontró el nodo padre con Id = {nodo.Valor.PadreId.Value} para la categoría: {nodo.Valor.Nombre}");
                    }
                }
                else
                {
                    raices.Add(nodo);
                }
            }
            return raices;
        }

        public async Task<List<int>> GetDescendantCategoryIdsAsync(int categoriaId)
        {
            // 1) Recuperar todas las categorías
            var categorias = await _categoriaRepository.GetAllAsync();
            // 2) Construir diccionario padre → lista de hijos
            var lookup = categorias
                .Where(c => c.PadreId.HasValue)
                .ToLookup(c => c.PadreId.Value, c => c.Id);

            // 3) BFS/DFS para recolectar IDs
            var resultado = new List<int>();
            var queue = new Queue<int>();
            queue.Enqueue(categoriaId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                resultado.Add(current);
                //LookUp: Coleccion de claves asociada con secuencia de elementos (mapa de una clave a múltiples valores) 
                foreach (var childId in lookup[current])
                    queue.Enqueue(childId);
            }
            return resultado;
        }
    }
}
