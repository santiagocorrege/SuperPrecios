using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SistemaAutenticacion.Exceptions.Email;

public class EmailException : Exception
{
    public EmailException()
    {
    }

    public EmailException(string? message) : base(message)
    {
    }

    public EmailException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected EmailException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
