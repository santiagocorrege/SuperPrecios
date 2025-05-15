using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Excepciones
{
    class SupermercadoException : Exception
    {
        public SupermercadoException()
        {
        }

        public SupermercadoException(string? message) : base(message)
        {
        }

        public SupermercadoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SupermercadoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
