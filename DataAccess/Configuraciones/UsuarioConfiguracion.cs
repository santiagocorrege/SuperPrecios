using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SistemaAutenticacion.Entidades;
using SistemaAutenticacion.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Papeleria.AccesoDatos.Configuraciones
{
    public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            var emailConverter = new ValueConverter<Email, string>
                (
                    e => e.Valor,
                    e => new Email(e)
                );
            builder.Property(u => u.Email).HasConversion(emailConverter);
        }
    }
}
