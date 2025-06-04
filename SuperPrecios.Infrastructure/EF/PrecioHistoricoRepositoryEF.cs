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
            if(precioHistorico == null) throw new ArgumentNullException("El precio historico no puede ser nulo", nameof(precioHistorico));
            try
            {
                var super = await _context.Supermercados.FindAsync(precioHistorico.SupermercadoId);
                if (super == null) throw new ArgumentException("El Supermercado no existe");
                var productoPrecioHistorico = precioHistorico.Producto;
                var productoBuscado = await _context.Productos
                    .AsNoTracking()
                    .Include(p => p.Categoria)
                    .Include(m => m.Marca)
                    .FirstOrDefaultAsync(p => p.Nombre == productoPrecioHistorico.Nombre && p.Marca.Nombre == productoPrecioHistorico.Marca.Nombre);                    

                if (productoBuscado != null)
                {
                    productoPrecioHistorico.Id = productoBuscado.Id;
                    _context.Entry(productoPrecioHistorico).State = EntityState.Unchanged;
                    _context.Entry(productoPrecioHistorico.Marca).State = EntityState.Unchanged;                    
                    _context.Entry(productoPrecioHistorico.Categoria).State = EntityState.Unchanged;                    
                }
                else
                {
                    var marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Nombre == productoPrecioHistorico.Marca.Nombre);
                    if (marca != null)
                    {
                        precioHistorico.Producto.Marca = marca;
                    }
                    else
                    {
                        await _context.Marcas.AddAsync(productoPrecioHistorico.Marca);
                    }
                    var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Nombre == productoPrecioHistorico.Categoria.Nombre);
                    if (categoria != null)
                    {
                        precioHistorico.Producto.Categoria = categoria;
                    }
                    else
                    {
                        await _context.Categorias.AddAsync(productoPrecioHistorico.Categoria);
                    }
                    var productoSolo = await _context.Productos.FirstOrDefaultAsync(p => p.Nombre == productoPrecioHistorico.Nombre);
                    if (productoSolo != null)
                    {
                        precioHistorico.Producto = productoSolo;
                    }
                    else
                    {
                        await _context.Productos.AddAsync(productoPrecioHistorico);
                    }
                }
                await _context.PreciosHistoricos.AddAsync(precioHistorico);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is SqlException sqlEx)
                {
                    // Error de índice único o duplicado de clave (2627 ó 2601)
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                    {
                        throw new Exception("Error de duplicado en la tabla");
                    }
                    // Error de violación de clave foránea (547)
                    if (sqlEx.Number == 547)
                    {
                        throw new Exception("Violación de clave foránea");
                    }
                }
                throw new Exception("Error al guardar el precio historico en la base de datos.");
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