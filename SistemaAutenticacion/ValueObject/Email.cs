using SistemaAutenticacion.InterfacesEntidades;
using SistemaAutenticacion.Exceptions.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAutenticacion.ValueObject
{
    [ComplexType]
    public record Email : IValidate
    {
        public string Valor { get; init; }

        public Email(string valor)
        {
            Valor = valor;
        }

        protected Email() { }
        public void Validate()
        {
            if (string.IsNullOrEmpty(Valor))
            {
                throw new EmailException("El email no puede ser nulo");
            }
            if (!Formato())
            {
                throw new EmailException("El formato del email no es valido");
            }
            
        }

        private bool Formato()
        {
            string patron = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            return Regex.IsMatch(Valor, patron);
        }

    }
}
