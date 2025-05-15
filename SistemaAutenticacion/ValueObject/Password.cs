using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SistemaAutenticacion.InterfacesEntidades;
using SistemaAutenticacion.Exceptions.Password;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaAutenticacion.ValueObject
{
    [ComplexType]
    public record Password
    {
        public string Valor { get; init; }

        public string Encriptada { get; init; }

        public Password (string valor)
        {
            Validate(valor);
            Valor = valor;
            Encriptada = Encrypt(valor);
        }

        protected Password() { }
        public static string Encrypt(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertir la contraseña en bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Calcular el hash de la contraseña
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convertir el hash a una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public void Validate(string password)
        {
            string patron = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[.,;!]).{6,}$";
            if (string.IsNullOrEmpty(password))
            {
                throw new PasswordException("La contrasena no puede ser nula");
            }
            if(!Regex.IsMatch(password, patron))
            {
                throw new PasswordException("La contrasena debe tener un largo mínimo de 6 caracteres, al menos una letra mayúscula, una minúscula, un dígito y un carácter de puntuación: punto, punto y coma, coma, signo de admiración de cierre");
            }
        }
    }
}
