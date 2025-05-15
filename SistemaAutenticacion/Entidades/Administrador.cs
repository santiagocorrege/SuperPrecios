using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAutenticacion.Entidades
{
    public class Administrador : Usuario
    {
        public Administrador(string nombre, string apellido, string email, string password) : base(nombre, apellido, email, password)
        {
            Validate();
        }

        protected Administrador() { }
        public override string Rol()
        {
            return "Administrador";
        }
    }
}
