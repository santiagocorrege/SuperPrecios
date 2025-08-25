
namespace SuperPrecios.Domain.Entities
{
    public class Proveedor : Usuario,IEntity, IValidate
    {
        #region Properties
        public Supermercado Supermercado { get; set; }
        public int? SupermercadoId { get; protected set; }

        public Proveedor(string nombre, string apellido, string email, string password, int? supermercadoId) : base(nombre, apellido, email, password)
        {
            SupermercadoId = supermercadoId ?? 0;
            ValidateProveedor();
        }

        public Proveedor(string nombre, string apellido, string email, string password, Supermercado supermercado) : base(nombre, apellido, email, password)
        {
            Supermercado = supermercado;
            ValidateProveedor();
        }

        public Proveedor(string nombre, string apellido, string email, string password) : base(nombre, apellido, email, password)
        {
            base.Validate();
        }


        public Proveedor(string nombre, string apellido, string email) : base(nombre, apellido, email)
        {
            base.Validate();
        }


        protected Proveedor() { }

        #endregion
        #region Methods
        public override string Rol()
        {
            return "Proveedor";
        }

        public void ValidateProveedor() {
            base.Validate();
            if ((SupermercadoId == null || SupermercadoId < 1) && Supermercado == null) throw new ArgumentException("Un proveedor debe tener un supermercado asignado");
        }
        #endregion
    }

}