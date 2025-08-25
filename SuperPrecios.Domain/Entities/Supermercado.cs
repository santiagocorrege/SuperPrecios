using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Exceptions;
using SuperPrecios.Shared;
using System.ComponentModel.DataAnnotations;


namespace SuperPrecios.Domain.Entities
{
    [Index(nameof(Nombre), IsUnique = true)]
    public class Supermercado : IEntity, IValidate
    {
        #region Properties
        public int Id { get; set; }
        public Proveedor? Proveedor { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Url]
        public string? WebsiteUrl { get; set; }        

        public Supermercado(string name, string websiteUrl)
        {
            Nombre = UtilidadesString.FormatearTexto(name);
            WebsiteUrl = websiteUrl;            
            Validate();
        }

        public Supermercado(string name)
        {
            Nombre = UtilidadesString.FormatearTexto(name);
            ValidateNombre();
        }

        protected Supermercado(){}
        #endregion

        #region Methods
        public void Validate()
        {
            ValidateNombre();
            if(String.IsNullOrWhiteSpace(WebsiteUrl))
            {
                throw new SupermercadoException("Error: La URL del supermercado no puede ser nula");
            }
        }

        public void ValidateNombre()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new SupermercadoException("Error: El nombre del supermercado no puede ser nulo");
            }
        }
        #endregion        

    }
}
