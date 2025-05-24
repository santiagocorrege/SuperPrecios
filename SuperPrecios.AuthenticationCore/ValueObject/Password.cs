using System;
using System.Text.RegularExpressions;
using BCrypt.Net;
using SuperPrecios.AuthenticationCore.Exceptions.Password;
using SuperPrecios.AuthenticationCore.InterfacesEntidades;

namespace SuperPrecios.AuthenticationCore.ValueObject
{
    //Complex Type por convencion (No tiene id y se relaciona con usuario)
    public record Password
    {
        public string Hash { get; init; }

        public Password(string plainPassword)
        {
            Validate(plainPassword);
            Hash = BCrypt.Net.BCrypt.HashPassword(plainPassword, 12); // Corregido
        }

        protected Password() { }

        public static Password FromHash(string hash)
        {
            return new Password { Hash = hash };
        }

        public bool Verify(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                return false;
            try
            {
                return BCrypt.Net.BCrypt.Verify(plainPassword, Hash);
            }
            catch
            {
                return false;
            }
        }

        private void Validate(string password)
        {
            string patron = @"^(?!\s)(?!.*\s$).{8,64}$";
            if (string.IsNullOrEmpty(password))
                throw new PasswordException("La contraseña no puede ser nula");
            if (!Regex.IsMatch(password, patron))
                throw new PasswordException("La contraseña debe tener un largo mínimo de 8 caracteres, no se permiten espacios");
        }

    }
}