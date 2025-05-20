using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Excepciones
{
    internal class ProductoHistoricoException : Exception
    {
        public ProductoHistoricoException()
        {
        }

        public ProductoHistoricoException(string? message) : base(message)
        {
        }

        public ProductoHistoricoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ProductoHistoricoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
