using Microsoft.EntityFrameworkCore;
using SuperPrecios.AuthenticationCore.ValueObject;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entities
{
    [Index(nameof(Nombre), IsUnique = true)]
    public class Marca : IEntity, IValidate, IEquatable<Marca>
    {            
        #region Properties
        public int Id { get; set; }

        public string Nombre { get; set; }

        public List<Producto> Productos { get; set; }

        public Marca(string nombre)
        {
            Nombre = UtilidadesString.FormatearTexto(nombre);
            Productos = new List<Producto>();
            Validate();
        }

        // Constructor sin parámetros, marcado como interno o privado
        // Solo para uso del ORM, no llama Validate.
        protected Marca() {
            Productos = new List<Producto>();
        }
        #endregion
        #region Methods
        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(Nombre))
            {
                throw new LocalException("Error: El nombre de la marca no puede ser nulo");
            }
        }

        public bool Equals(Marca? other)
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
