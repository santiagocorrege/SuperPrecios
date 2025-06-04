using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.AuthenticationCore.Entities
{
    public class Proveedor : Usuario
    {
        public Proveedor(string nombre, string apellido, string email, string password) : base(nombre, apellido, email, password)
        {
            Validate();
        }
        public override string Rol()
        {
            return "Proveedor";
        }
    }
}
