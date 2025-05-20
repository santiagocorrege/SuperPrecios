using LogicaNegocio.Excepciones;
using Prueba.Entidades;
using Prueba.InterfacesEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Entidades
{
    public class Supermercado : IEntity, IValidate
    {
        #region Properties
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Url]
        public string WebsiteUrl { get; set; }

        public virtual ICollection<PrecioHistorico> PreciosHistoricos { get; set; }

        public Supermercado(string name, string websiteUrl)
        {
            Name = name;
            WebsiteUrl = websiteUrl;
            PreciosHistoricos = new List<PrecioHistorico>();
            Validate();
        }

        protected Supermercado(){}
        #endregion

        #region Methods
        public void Validate()
        {
            if(string.IsNullOrWhiteSpace(Name))
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
