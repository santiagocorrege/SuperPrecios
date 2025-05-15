using SistemaAutenticacion.InterfacesEntidades;
using SistemaAutenticacion.ValueObject;
using SistemaAutenticacion.Exceptions.Usuario;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SistemaAutenticacion.Entidades
{
    [Index(nameof(Email), IsUnique = true)]
    public abstract class Usuario : IEntity, IValidate
    {
        
        #region Properties
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Apellido { get; set; }
        
        public Email Email { get; init; }
        
        public Password Password { get; set; }

        public Usuario(string nombre, string apellido, string email, string password)
        {
            Nombre = nombre;
            Apellido = apellido;
            Email = new Email(email);
            Password = new Password(password);
            Validate();
        }
        #endregion

        protected Usuario() { }
        #region Methods
        public void Validate()
        {
            //El nombre y el apellido solamente pueden contener caracteres alfabéticos, espacio, apóstrofe o guión del medio.
            //Los caracteres no alfabéticos no pueden estar ubicados al principio ni al final de la cadena
            string patron = @"^[a-zA-Z]+(?:[' -][a-zA-Z]+)*$";
            //Chequeo el caso de que sea nulo ya que el Regex chequearia si fuese vacio.
            if (string.IsNullOrEmpty(Nombre))
            {
                throw new UsuarioException("El nombre no puede ser nulo");
            }
            if (string.IsNullOrEmpty(Apellido))
            {
                throw new UsuarioException("El apellido no puede ser nulo");
            }
            if (!Regex.IsMatch(Nombre, patron))
            {
                throw new UsuarioException("El formato del nombre no es valido ");
            }
            if (!Regex.IsMatch(Apellido, patron))
            {
                throw new UsuarioException("El formato del apellido no es valido ");
            }
        }

        public void Modificar(Usuario usuario)
        {
            Nombre = usuario.Nombre;
            Apellido = usuario.Apellido;
            Password = usuario.Password;
        }

        public abstract string Rol();

        #endregion
    }
}
