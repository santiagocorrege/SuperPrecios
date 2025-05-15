using Comun;
using Prueba.Excepciones;
using Prueba.InterfacesEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Entidades
{
    public class Marca : IEntity, IValidate
    {
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
        public void Validate()
        {
            if (String.IsNullOrWhiteSpace(Nombre))
            {
                throw new LocalException("Error: El nombre de la marca no puede ser nulo");
            }
        }
    }
}
