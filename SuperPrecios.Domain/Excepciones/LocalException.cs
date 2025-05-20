using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Excepciones
{
    public class LocalException : Exception
    {
        public LocalException()
        {
        }

        public LocalException(string? message) : base(message)
        {
        }

        public LocalException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected LocalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
