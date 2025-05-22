using SuperPrecios.AutenticacionCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IRepository
{
    public interface IUsuarioRepository
    {
        public Task<Usuario> GetByUsuarioLogin(string email, string password);
    }
}
