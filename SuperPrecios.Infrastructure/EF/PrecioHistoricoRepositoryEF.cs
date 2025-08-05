using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.IRepositories;
using System.Data.Common;


namespace SuperPrecios.Infrastructure.EF
{
    public class PrecioHistoricoRepositoryEF : IPrecioHistoricoRepository
    {
        private readonly SuperPreciosDbContext _context;

        public PrecioHistoricoRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }
        //Descontinuado
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
                        //await _context.Categorias.AddAsync(productoNuevo.Categoria);
                        throw new Exception("La categoria ingresada no existe");
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


        public async Task AddAsyncBySupermercadoAndCategoria(IEnumerable<PrecioHistorico> preciosHistoricos, Supermercado supermercado, Categoria categoria)
        {
            // Validaciones iniciales
            if (preciosHistoricos == null || !preciosHistoricos.Any())
                throw new ArgumentException(
                    "La lista de precios historicos no puede ser nula o vacía.",
                    nameof(preciosHistoricos));
            if (supermercado == null)
                throw new ArgumentNullException(nameof(supermercado));
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            foreach (var precioHistorico in preciosHistoricos)
            {
                try
                {
                    // Forzar referencias correctas (por si vienen mal en el DTO)
                    precioHistorico.Supermercado = supermercado;
                    precioHistorico.Producto.Categoria = categoria;

                    // --- Lógica de producto/marca igual al caso individual ---
                    var productoNuevo = precioHistorico.Producto;

                    // 1) ¿Existe un producto con mismo nombre y marca?
                    var productoExistente = await _context.Productos
                        .Include(p => p.Marca)
                        .Include(p => p.Categoria)
                        .FirstOrDefaultAsync(p =>
                            p.Nombre == productoNuevo.Nombre &&
                            p.Marca.Nombre == productoNuevo.Marca.Nombre);

                    if (productoExistente != null)
                    {
                        precioHistorico.Producto = productoExistente;
                    }
                    else
                    {
                        // 2) Marca
                        var marcaExistente = await _context.Marcas
                            .FirstOrDefaultAsync(m => m.Nombre == productoNuevo.Marca.Nombre);
                        if (marcaExistente != null)
                            productoNuevo.Marca = marcaExistente;
                        else
                            await _context.Marcas.AddAsync(productoNuevo.Marca);

                        // 3) Categoría (ya validada arriba)
                        productoNuevo.Categoria = categoria;

                        // 4) Verificar producto por nombre
                        var productoPorNombre = await _context.Productos
                            .FirstOrDefaultAsync(p => p.Nombre == productoNuevo.Nombre);
                        if (productoPorNombre != null)
                            precioHistorico.Producto = productoPorNombre;
                        else
                            await _context.Productos.AddAsync(productoNuevo);
                    }

                    // --- Guardar el PrecioHistórico ---
                    await _context.PreciosHistoricos.AddAsync(precioHistorico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    if (dbEx.InnerException is SqlException sqlEx)
                    {
                        if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        {
                            Console.WriteLine(
                                $"[WARN] Duplicado detectado: Producto \"{precioHistorico.Producto.Nombre}\" " +
                                $"en SupermercadoId={precioHistorico.Supermercado.Id}. " +
                                $"SQL Error {sqlEx.Number}: {sqlEx.Message}");
                            continue;
                        }
                        if (sqlEx.Number == 547)
                        {
                            Console.WriteLine(
                                $"[WARN] Violación de FK al insertar PrecioHistorico para " +
                                $"Producto \"{precioHistorico.Producto.Nombre}\". " +
                                $"SQL Error {sqlEx.Number}: {sqlEx.Message}");
                            continue;
                        }
                    }

                    // Caso genérico de DbUpdateException
                    Console.WriteLine(
                        $"[ERROR] Error de base de datos al guardar PrecioHistorico: " +
                        $"Producto=\"{precioHistorico.Producto.Nombre}\", " +
                        $"Supermercado=\"{precioHistorico.Supermercado.Nombre}\". " +
                        $"Excepción: {dbEx.Message}");
                    continue;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"[ERROR] Error inesperado al procesar PrecioHistorico para " +
                        $"Producto=\"{precioHistorico.Producto.Nombre}\". " +
                        $"Excepción: {ex.Message}");
                    continue;
                }
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