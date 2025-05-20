using Microsoft.EntityFrameworkCore;
using SuperPrecios.Domain.Excepciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Domain.Entidades
{
    [PrimaryKey(nameof(ProductoId), nameof(Fecha))]
    public class PrecioHistorico : IEntity, IValidate
    {
        #region Properties
        public int Id { get; set; }

        public Producto? Producto { get; set; }

        [Required]
        [ForeignKey(nameof(ProductoId))]
        public int ProductoId { get; set; }        
                           
        public Supermercado Supermercado { get; set; }

        [ForeignKey(nameof(SupermercadoId))]
        public int SupermercadoId { get; set; }

        [Required]
        public DateOnly Fecha {  get; set; }
        [Required]
        public decimal Precio { get; set; }

        public PrecioHistorico(int productoId, int supermercadoId, decimal precio) {
            ProductoId = productoId;
            SupermercadoId = supermercadoId;
            Precio = precio;
            Fecha = DateOnly.FromDateTime(DateTime.UtcNow);
            Validate();
        }

        internal PrecioHistorico() { }

        #endregion
        #region Methods
        public void Validate()
        {
            if(ProductoId == 0 || Producto == null)
            {
                throw new ProductoHistoricoException("Error: No se puede guardar un registro de precio sin un producto relacionado");
            }
            if(Precio <= 0 || Precio >= decimal.MaxValue)
            {
                throw new ProductoHistoricoException("Error: El precio no puede ser menor o igual a 0");
            }
        }
        #endregion
    }
}
