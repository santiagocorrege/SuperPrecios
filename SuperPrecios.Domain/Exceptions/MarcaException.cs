using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Exceptions
{
    public class MarcaException : Exception
    {
        public MarcaException()
        {
        }

        public MarcaException(string? message) : base(message)
        {
        }

        public MarcaException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MarcaException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
