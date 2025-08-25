using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperPrecios.Domain.Entities
{
    public class Carrito : IEntity, IEquatable<Carrito>
    {
        #region Properties
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public ICollection<Linea> Lineas { get; set; }        
        public DateTime FechaCreacion { get; set; }
        
        #endregion

        #region Constructors        
        protected Carrito()
        {
            Lineas = new List<Linea>();
            FechaCreacion = DateTime.Now;
        }

        public Carrito(Usuario usuario) : this()
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            UsuarioId = usuario.Id;
        }

        public Carrito(int usuarioId) : this()
        {
            if (usuarioId <= 0) throw new ArgumentException("El ID del usuario debe ser mayor a 0", nameof(usuarioId));
            UsuarioId = usuarioId;
        }
        #endregion

        #region Methods
        public void AgregarLinea(Linea linea)
        {
            if (linea == null) throw new ArgumentNullException(nameof(linea));

            linea.Validate();            
            var lineaExistente = Lineas.FirstOrDefault(l =>
                l.ProductoId == linea.ProductoId);

            if (lineaExistente != null)
            {
                lineaExistente.ModificarCantidad(1);
            }
            else
            {                
                linea.CarritoId = this.Id;
                linea.Carrito = this;
                Lineas.Add(linea);
            }
        }

        public void QuitarLineaPorProducto(int productoId)
        {
            var linea = Lineas.FirstOrDefault(l => l.ProductoId == productoId);
            if (linea != null)
            {
                Lineas.Remove(linea);
            }
        }

        public void ModificarCantidadLinea(int idProducto, int cantidad)
        {
            var lineaEncontrada = BuscarLinea(idProducto);
            if(lineaEncontrada != null)
            {
                if(lineaEncontrada.Cantidad + cantidad <= 0)
                {
                    Lineas.Remove(lineaEncontrada);
                }
                else
                {
                    lineaEncontrada.ModificarCantidad(cantidad);
                }                    
            }
            else
            {
                throw new Exception("La linea no existe en el carrito");
            }
            
        }

        public void LimpiarCarrito()
        {
            Lineas.Clear();
        }

        private Linea? BuscarLinea(int idProducto)
        {
            if (idProducto <= 0) return null;
            return Lineas.FirstOrDefault(l => l.ProductoId == idProducto );
        }

        public void Validate()
        {
            if (UsuarioId <= 0)
                throw new ArgumentException("ID de usuario inválido - debe ser mayor a 0");

            if (Usuario != null && Usuario.Id != UsuarioId)
                throw new InvalidOperationException("Inconsistencia entre Usuario y UsuarioId");
        }

        public List<Producto> GetProductos()
        {
            List<Producto> productos = new List<Producto>();
            foreach(Linea l in Lineas)
            {
                productos.Add(l.Producto);
            }
            return productos;
        }
        public bool Equals(Carrito? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Id > 0; 
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Carrito);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}