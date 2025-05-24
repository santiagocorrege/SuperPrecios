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
    [PrimaryKey(nameof(ProductoId), nameof(SupermercadoId), nameof(Fecha))]
    public class PrecioHistorico : IValidate
    {
        #region Properties
        public Producto? Producto { get; set; }

        [Required]        
        public int ProductoId { get; set; }        
                           
        public Supermercado Supermercado { get; set; }

        [ForeignKey(nameof(SupermercadoId))]
        public int SupermercadoId { get; set; }

        [Required]
        public DateOnly Fecha {  get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
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
            if(Producto == null || ProductoId <= 0 )
            {
                throw new ProductoHistoricoException("Error: No se puede guardar un registro de precio sin un producto relacionado");
            }
            if(Supermercado == null || SupermercadoId <= 0)
            {
                throw new ProductoHistoricoException("Error: No se puede guardar un registro de precio sin un supermercado relacionado");
            }
            if (Precio <= 0 || Precio >= decimal.MaxValue)
            {
                throw new ProductoHistoricoException("Error: El precio no puede ser menor o igual a 0");
            }
        }
        #endregion
    }
}
