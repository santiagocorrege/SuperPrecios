using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.TAD
{
    public class Nodo<T> : IEquatable<Nodo<T>>
    {
        public T Valor { get; set; }

        public int Id { get; set; }

        public List<Nodo<T>> Siguientes { get; set; }

        public Nodo(T valor, int id)
        {
            Valor = valor;
            Id = Id;
            Siguientes = new List<Nodo<T>>();
        }

        public void AgregarHijo(Nodo<T> nodo)
        {
            if (nodo == null) throw new Exception("El nodo no puede ser nulo");
            Siguientes.Add(nodo);
        }

        public bool Equals(Nodo<T>? other)
        {
            if (other == null) throw new ArgumentNullException("Los valores no pueden ser nulos");
            return Id.Equals(other.Id);
        }

        
        public override string ToString()
        {
            return Valor?.ToString() ?? "null";
        }

        public override bool Equals(object? obj)
        {
            return obj is Nodo<T> nodo && Equals(nodo);
        }

        public override int GetHashCode()
        {
            return Valor?.GetHashCode() ?? 0;
        }
    }
}
