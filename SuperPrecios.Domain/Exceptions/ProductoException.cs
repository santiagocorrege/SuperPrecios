using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Excepciones
{
    public class ProductoException : Exception
    {
        public ProductoException()
        {
        }

        public ProductoException(string? message) : base(message)
        {
        }

        public ProductoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ProductoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
