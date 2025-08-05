using SuperPrecios.Application.IRepository;
using SuperPrecios.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.IRepositories
{
    public interface IPrecioHistoricoRepository
    {
        // Métodos existentes (mantenidos por compatibilidad)
        Task AddAsync(PrecioHistorico precioHistorico);
        Task AddAsyncBySupermercadoAndCategoria(IEnumerable<PrecioHistorico> preciosHistoricos, Supermercado supermercado, Categoria categoria);
        Task<IEnumerable<Producto>> GetAllBySupermercado(int supermercadoId);
        Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, int productoId);
        Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, string productoNombre);

        // ✅ NUEVO: Método optimizado para IDs sincronizados
        /// <summary>
        /// Método de alta performance para insertar precios históricos con IDs sincronizados.
        /// Usa inserción en lote y validación optimizada.
        /// </summary>
        /// <param name="preciosHistoricos">Lista de precios históricos con IDs ya asignados</param>
        /// <returns>Task con resultado de la operación</returns>
        Task AddAsyncBulkWithSyncedIds(IEnumerable<PrecioHistorico> preciosHistoricos);
    }
}
