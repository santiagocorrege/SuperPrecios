using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entidades
{
    public class Marca : IEntity, IValidate
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
        #endregion
    }
}
