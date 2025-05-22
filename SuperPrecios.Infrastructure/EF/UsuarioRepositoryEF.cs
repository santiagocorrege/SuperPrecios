using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.IRepository;
using SuperPrecios.AutenticacionCore.Entities;
using SuperPrecios.AutenticacionCore.Exceptions.Usuario;
using SuperPrecios.AutenticacionCore.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Infrastructure.EF
{
    public class UsuarioRepositoryEF : IUsuarioRepository
    {
        private readonly SuperPreciosDbContext _context;

        public UsuarioRepositoryEF(SuperPreciosDbContext context)
        {
            _context = context;
        }
        public async Task<Usuario> GetByUsuarioLogin(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    throw new ArgumentException("El usuario y/o contrasenas no pueden ser nulos");
                }
                string pass = Password.Encrypt(password);
                Usuario usuario = await _context.Usuarios.Where(u => u.Email.Valor == email && u.Password.Encriptada.Equals(pass)).FirstOrDefaultAsync();
                if (usuario == null) throw new UsuarioException($"Usuario y/o password invalidos");
                return usuario;
            }
            catch (Exception ex)
            {
                // Captura cualquier otra excepción inesperada
                throw new Exception("DB: Error : Error al acceder a la base de datos durante la búsqueda por email", ex);
            }
        }

    }
}
