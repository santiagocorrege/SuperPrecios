using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SuperPrecios.Application.IRepository;
using SuperPrecios.Domain.Entities;
using SuperPrecios.Domain.Entities.ValueObject;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
            catch (DbException ex)
            {
                throw new Exception("BD Error: al consultar la base de datos de miembros");
            }

        }

    }
}
