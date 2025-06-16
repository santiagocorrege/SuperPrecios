using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.IServices.PrecioHistorico;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class PrecioHistoricoRepositoryEF : IPrecioHistoricoRepository
    {
        private readonly SuperPreciosDbContext _context;

        public PrecioHistoricoRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PrecioHistorico precioHistorico)
        {
            if (precioHistorico == null)
                throw new ArgumentNullException(nameof(precioHistorico), "El precio histórico no puede ser nulo");

            try
            {
                // Validar supermercado
                var supermercado = await _context.Supermercados.FindAsync(precioHistorico.SupermercadoId);
                if (supermercado == null)
                    throw new ArgumentException("El supermercado no existe");

                // Acceso al producto enviado
                var productoNuevo = precioHistorico.Producto;

                // Buscar si ya existe un producto con mismo nombre y marca
                var productoExistente = await _context.Productos
                    .Include(p => p.Marca)
                    .Include(p => p.Categoria)
                    .FirstOrDefaultAsync(p =>
                        p.Nombre == productoNuevo.Nombre &&
                        p.Marca.Nombre == productoNuevo.Marca.Nombre);

                if (productoExistente != null)
                {
                    // Reusar el producto existente
                    precioHistorico.Producto = productoExistente;
                }
                else
                {
                    // Buscar o agregar la marca
                    var marcaExistente = await _context.Marcas.FirstOrDefaultAsync(m => m.Nombre == productoNuevo.Marca.Nombre);
                    if (marcaExistente != null)
                    {
                        productoNuevo.Marca = marcaExistente;
                    }
                    else
                    {
                        await _context.Marcas.AddAsync(productoNuevo.Marca);
                    }

                    // Buscar o agregar la categoría
                    var categoriaExistente = await _context.Categorias.FirstOrDefaultAsync(c => c.Nombre == productoNuevo.Categoria.Nombre);
                    if (categoriaExistente != null)
                    {
                        productoNuevo.Categoria = categoriaExistente;
                    }
                    else
                    {
                        await _context.Categorias.AddAsync(productoNuevo.Categoria);
                    }

                    // Verificar si el producto (sin considerar marca) ya existe
                    var productoPorNombre = await _context.Productos
                        .FirstOrDefaultAsync(p => p.Nombre == productoNuevo.Nombre);

                    if (productoPorNombre != null)
                    {
                        // En ese caso, se asume que es el mismo
                        precioHistorico.Producto = productoPorNombre;
                    }
                    else
                    {
                        // Es un producto nuevo completo
                        await _context.Productos.AddAsync(productoNuevo);
                    }
                }

                // Guardar el precio histórico
                await _context.PreciosHistoricos.AddAsync(precioHistorico);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        throw new Exception("Error de duplicado en la tabla");

                    if (sqlEx.Number == 547)
                        throw new Exception("Violación de clave foránea");
                }

                throw new Exception("Error al guardar el precio histórico en la base de datos.");
            }
        }


        public async Task<IEnumerable<Producto>> GetAllBySupermercado(int supermercadoId)
        {
            if (supermercadoId <= 0) throw new ArgumentException("El id del supermercado no puede ser menor o igual a 0");
            try
            {
                var super = await _context.Supermercados.FindAsync(supermercadoId);
                if (super == null) throw new ArgumentNullException("El Supermercado no existe");
                var productos = await _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Marca)
                .Include(p => p.PreciosHistoricos.Where(ph => ph.SupermercadoId == supermercadoId))
                .ToListAsync();
                return productos;
            }
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }
        }



        public async Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, int productoId)
        {
            if (productoId <= 0 || supermercadoId <= 0) throw new ArgumentException("Id de producto y/o supermercado invalido");
            try
            {
                var super = await _context.Supermercados.FindAsync(supermercadoId);
                if (super == null) throw new ArgumentNullException("El Supermercado no existe");
                return await _context.PreciosHistoricos
                         .AsNoTracking()
                         .Where(ph => ph.Producto.Id == productoId
                                   && ph.SupermercadoId == supermercadoId)
                         .ToListAsync();
            }catch (DbUpdateException dbEx)
            {
                throw new Exception("Error al buscar el precio historico del producto en la base de datos.", dbEx);
            }

        }

        public async Task<IEnumerable<PrecioHistorico>> GetPrecioHistoricoProductoBySupermercado(int supermercadoId, string productoNombre)
        {
            if (String.IsNullOrWhiteSpace(productoNombre) || supermercadoId <= 0) throw new ArgumentException("Id de producto y/o nombre supermercado invalido");
            try
            {
                var super = await _context.Supermercados.FindAsync(supermercadoId);
                if (super == null) throw new ArgumentNullException("El Supermercado no existe");                
                return await _context.PreciosHistoricos
                         .AsNoTracking()
                         .Where(ph =>
                         ph.Producto.Nombre == productoNombre &&
                         ph.SupermercadoId == supermercadoId
                         )
                         .ToListAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("Error al buscar el supermercado en la base de datos.", dbEx);
            }
        }


    }
}