using Microsoft.EntityFrameworkCore;
using System;

namespace SuperPrecios.Domain.Entities
{
    [PrimaryKey(nameof(CarritoId), nameof(ProductoId))]
    public class Linea : IValidate, IEquatable<Linea>
    {
        #region Properties
        public Carrito Carrito { get; set; }
        public int CarritoId { get; set; }
        public Producto Producto { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        #endregion

        #region Constructors
        protected Linea() { }

        public Linea(int idProducto, int idCarrito, int cantidad)
        {
            ProductoId = idProducto;
            CarritoId = idCarrito;
            Cantidad = cantidad;
            Validate();
        }

        public Linea(Producto producto, Carrito carrito, int cantidad)
        {
            Producto = producto ?? throw new ArgumentNullException(nameof(producto));
            Carrito = carrito ?? throw new ArgumentNullException(nameof(carrito));
            ProductoId = producto.Id; // Asumiendo que Producto tiene una propiedad Id
            CarritoId = carrito.Id;   // Asumiendo que Carrito tiene una propiedad Id
            Cantidad = cantidad;
            Validate();
        }
        #endregion

        #region Methods

        public void ModificarCantidad(int cantidad)
        {
            
            if (cantidad > 0 && cantidad <= 1000)
            {
                Cantidad = cantidad;
            }
            else
            {
                throw new Exception("Cantidad no valida para la linea");
            }
        }
        public void Validate()
        {
            if (Producto != null && Carrito != null)
            {
                Carrito.Validate();
                Producto.Validate();
            }
            else if (ProductoId <= 0 || CarritoId <= 0)
            {
                throw new ArgumentException("Producto o Carrito no válido - Los IDs deben ser mayores a 0");
            }

            if (Cantidad <= 0)
            {
                throw new ArgumentException("La cantidad de la línea no puede ser menor a 1");
            }
        }

        public bool Equals(Linea? other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;

            // Primero comparar por entidades si están disponibles
            if (Producto != null && other.Producto != null &&
                Carrito != null && other.Carrito != null)
            {
                return Producto.Equals(other.Producto) && Carrito.Equals(other.Carrito);
            }

            // Si no, comparar por IDs
            return ProductoId == other.ProductoId && CarritoId == other.CarritoId;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Linea);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProductoId, CarritoId);
        }

        #endregion
    }
}