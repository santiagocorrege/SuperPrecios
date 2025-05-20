using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Excepciones;
using SuperPrecios.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entidades
{    
    public class Producto : IEntity, IValidate
    {
        #region Properties
        public int Id { get; set; }
        public string Nombre { get; set; }
              
        public Marca Marca { get; set; }

        [ForeignKey(nameof(Marca))]
        public int MarcaId { get; set; }

        public Supermercado Supermercado { get; set; }

        [ForeignKey(nameof(Supermercado))]
        public int SupermercadoId { get; set; }
        public Categoria Categoria { get; set; }

        [ForeignKey(nameof(CategoriaId))]
        public int CategoriaId {  get; set; }

        public List<PrecioHistorico> PreciosHistoricos { get; set; }
        

        public Producto(string nombre, int nombreMarca, int nombreSupermercado, decimal precio)
        {
            PreciosHistoricos = new List<PrecioHistorico>();
            Nombre = UtilidadesString.FormatearTexto(nombre);            
            var precioHistorico = new PrecioHistorico
            {
                Fecha = DateOnly.FromDateTime(DateTime.UtcNow),
                Precio = precio,
                Producto = this // Establecemos la relación
            };
            precioHistorico.Validate();            
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
            if (MarcaId <= 0)
            {
                throw new ProductoException("Error: El nombre del producto no puede ser nulo");
            }
        }
        #endregion
    }
}
