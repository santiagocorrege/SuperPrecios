using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SuperPrecios.AuthenticationCore.Entities;
using SuperPrecios.AuthenticationCore.ValueObject;
using System.Reflection.Emit;

namespace SuperPrecios.Infrastructure.Configuraciones
{
    public class UsuarioConfiguracion : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            //builder.ComplexProperty(u => u.Email);
            //builder.ComplexProperty(u => u.Password);

            builder.Property(u => u.Email)
                .HasConversion(
                    e => e.Valor,
                    s => new Email(s)
                );

            builder.Property(u => u.Password)
                .HasConversion(
                    p => p.Hash,
                    h => Password.FromHash(h)
                );
        }
    }
}