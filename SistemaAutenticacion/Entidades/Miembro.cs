using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaAutenticacion.ValueObject;

namespace SistemaAutenticacion.Entidades
{
    public class Miembro : Usuario
    {
        public Miembro(string nombre, string apellido, string email, string password) : base(nombre, apellido, email, password)
        {
            Validate();
        }

       protected Miembro() { }

        public override string Rol()
        {
            return "Miembro";
        }
    }
}
