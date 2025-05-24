using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.AuthenticationCore.Exceptions
{
    public class MiembroException : Exception
    {
        public MiembroException()
        {
        }

        public MiembroException(string? message) : base(message)
        {
        }

        public MiembroException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected MiembroException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
