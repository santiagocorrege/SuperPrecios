using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entities
{
    [Index(nameof(Nombre), nameof(MarcaId), IsUnique = true)]

    public class Producto : IEntity, IValidate
    {
        #region Properties
        public int Id { get; set; }
        public string Nombre { get; set; }              
        public Marca Marca { get; set; }        
        public int MarcaId { get; set; }
        public Categoria Categoria { get; set; }       
        public int CategoriaId {  get; set; }

        public List<PrecioHistorico> PreciosHistoricos { get; set; }
        
        public Producto(string nombre, int marcaId, int categoriaId)
        {
            PreciosHistoricos = new List<PrecioHistorico>();
            Nombre = UtilidadesString.FormatearTexto(nombre);
            MarcaId = marcaId;
            CategoriaId = categoriaId;
            Validate();
        }

        public Producto(string nombre, Marca marca, Categoria categoria)
        {
            PreciosHistoricos = new List<PrecioHistorico>();
            Nombre = UtilidadesString.FormatearTexto(nombre);
            Marca = marca;
            Categoria = categoria;                        
            Validate();
        }

        protected Producto() {
            PreciosHistoricos = new List<PrecioHistorico>();
        }
        #endregion
        #region Methods
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new ProductoException("Error: El nombre del producto no puede ser nulo");
            }
            if (Marca == null && MarcaId <= 0)
            {
                throw new ProductoException("Error: El nombre del producto no puede ser nulo");
            }    
            if(Categoria == null && CategoriaId <= 0)
            {
                throw new ProductoException("Error: La categoria del producto no puede ser nula");
            }
        }

        public void Update(Producto producto)
        {
            if (producto == null)
            {
                throw new ArgumentNullException(nameof(producto), "El producto no puede ser nulo");
            }
            Nombre = UtilidadesString.FormatearTexto(producto.Nombre);
            MarcaId = producto.MarcaId;
            CategoriaId = producto.CategoriaId;
            Validate();
        }


        #endregion
    }
}
