using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Exceptions
{
    public class ProveedorException : Exception
    {
        public ProveedorException()
        {
        }

        public ProveedorException(string? message) : base(message)
        {
        }

        public ProveedorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ProveedorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
