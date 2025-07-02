using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entities
{
    [Index(nameof(Nombre))]
    public class Categoria : IEntity, IValidate, IEquatable<Categoria>
    {
        #region Properties
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<Producto> Productos { get; set; }

        [ForeignKey(nameof(Categoria))]
        public int? PadreId { get; set; }

        public Categoria? Padre { get; set; }

        public List<Categoria> Hijos { get; set; } = new();

        public Categoria(string nombre)
        {            
            Nombre = UtilidadesString.FormatearTexto(nombre);
            Productos = new List<Producto>();
        }
        //Test
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
        public bool Equals(Categoria? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            // Si ambos tienen Id asignado (> 0), comparar por Id
            if (Id > 0 && other.Id > 0)
                return this.Id == other.Id;

            // En caso contrario, comparar por Nombre (ignorando mayúsculas/minúsculas)
            return string.Equals(
                Nombre,
                other.Nombre                
            );
        }
        #endregion
    }
}
