using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entities
{
    public class Supermercado : IEntity, IValidate
    {
        #region Properties
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Url]
        public string WebsiteUrl { get; set; }        

        public Supermercado(string name, string websiteUrl)
        {
            Nombre = UtilidadesString.FormatearTexto(name);
            WebsiteUrl = websiteUrl;            
            Validate();
        }

        public Supermercado(string name)
        {
            Nombre = UtilidadesString.FormatearTexto(name);            
            Validate();
        }

        protected Supermercado(){}
        #endregion

        #region Methods
        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(Nombre))
            {
                throw new SupermercadoException("Error: El nombre del supermercado no puede ser nulo");
            }
            if(string.IsNullOrEmpty(WebsiteUrl))
            {
                throw new SupermercadoException("Error: La URL del supermercado no puede ser nula");
            }
        }
        #endregion        

    }
}
