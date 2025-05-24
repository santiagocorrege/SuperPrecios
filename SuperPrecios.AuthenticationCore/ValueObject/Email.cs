using SuperPrecios.AuthenticationCore.InterfacesEntidades;
using SuperPrecios.AuthenticationCore.Exceptions.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperPrecios.AuthenticationCore.ValueObject
{
    //Complex Type por convencion (No tiene id y se relaciona con usuario)
    public record Email : IValidate
    {
        public string Valor { get; init; }

        public Email(string valor)
        {
            Valor = valor;
            Validate();
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
                throw new EmailException("Por favor, ingresá una dirección de correo electrónico válida. Por ejemplo: nombre@ejemplo.com ");
            }
            
        }

        private bool Formato()
        {
            string patron = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            return Regex.IsMatch(Valor, patron);
        }

    }
}
