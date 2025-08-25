

namespace SuperPrecios.Domain.Entities
{
    public class Miembro : Usuario
    {
        public Carrito? Carrito { get; set; }
        
        public Miembro(string nombre, string apellido, string email, string password) : base(nombre, apellido, email, password)
        {            
            Validate();
        }

        public Miembro(string nombre, string apellido, string email) : base(nombre, apellido, email)
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
