using System.Collections.Generic;
using System.Threading.Tasks;
using SuperPrecios.Domain.Entities;

namespace SuperPrecios.Domain.IRepositories
{
    /// <summary>
    /// Interfaz para operaciones de matching transaccionales
    /// Respeta Clean Architecture - solo tipos del Domain
    /// </summary>
    public interface IMatchingRepository
    {
        /// <summary>
        /// Ejecuta todas las operaciones de matching en una sola transacción
        /// Si algo falla, se hace rollback completo
        /// </summary>
        Task ProcessMatchingTransactionAsync(
            IEnumerable<Marca> marcasNuevas,
            IEnumerable<Producto> productosNuevos,
            IEnumerable<PrecioHistorico> preciosHistoricos);

        /// <summary>
        /// Obtiene IDs de marcas existentes para evitar consultas N+1
        /// </summary>
        Task<HashSet<int>> GetMarcasExistentesByIdsAsync(IEnumerable<int> marcasIds);

        /// <summary>
        /// Obtiene IDs de productos existentes para evitar consultas N+1
        /// </summary>
        Task<HashSet<int>> GetProductosExistentesByIdsAsync(IEnumerable<int> productosIds);

        /// <summary>
        /// Valida que todos los supermercados y categorías existan
        /// </summary>
        Task ValidateSupermercadosAndCategoriasAsync(IEnumerable<int> supermercadosIds, IEnumerable<int> categoriasIds);
    }
}