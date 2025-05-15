using LogicaNegocio.Entidades;
using Prueba.Entidades;
using SistemaAutenticacion.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EntityFramework
{
    class SuperPreciosContext : DbContext
    {
        public DbSet<Supermercado> Supermercados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<PrecioHistorico> PreciosHistoricos { get; set; }
        
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Administrador> Administradores { get; set; }

        public DbSet<Miembro> Miembros { get; set; }       

    }
}
