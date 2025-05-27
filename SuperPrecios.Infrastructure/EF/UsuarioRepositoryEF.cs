using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.IRepository;
using SuperPrecios.AuthenticationCore.Entities;
using SuperPrecios.AuthenticationCore.Exceptions.Email;
using SuperPrecios.AuthenticationCore.Exceptions.Usuario;
using SuperPrecios.AuthenticationCore.ValueObject;
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
        public async Task<Usuario> GetByUsuarioLogin(string stringEmail, string plainPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stringEmail) || string.IsNullOrWhiteSpace(plainPassword))
                {
                    throw new ArgumentException("El usuario y/o contrasenas no pueden ser nulos");
                }
                Email email = new Email(stringEmail);
                var usuario = await _context.Usuarios
	            .Where(u => u.Email == email)
	            .FirstOrDefaultAsync();
				
				if (usuario != null && usuario.Password.Verify(plainPassword) == true)
                {                    
					return usuario;
				}                    
                return null;
            }            
            catch (EmailException ex) 
            {
                throw; 
            }
            catch (Exception ex)
            { 
                throw new Exception("DB: Error : Error al acceder a la base de datos durante la búsqueda por email", ex);
            }
        }

    }
}
