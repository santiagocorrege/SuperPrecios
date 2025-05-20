using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entidades
{
    [Index(nameof(Nombre), IsUnique = true)]
    public class Categoria : IEntity, IValidate
    {
        #region Properties
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<Producto> Productos { get; set; }

        public Categoria(string nombre)
        {            
            Nombre = UtilidadesString.FormatearTexto(nombre);
            Productos = new List<Producto>();
        }

        protected Categoria()
        {
            Productos = new List<Producto>();
        }
        #endregion
        #region Methods
        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(Nombre.Trim()))
            {
                throw new CategoriaException("Error: El nombre de la categoria no puede ser vacio");
            }
        }
        #endregion
    }
}
