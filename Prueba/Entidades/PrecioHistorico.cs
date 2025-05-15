using Microsoft.EntityFrameworkCore;
using Prueba.Excepciones;
using Prueba.InterfacesEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Entidades
{
    [Index(nameof(ProductoId), nameof(Fecha), IsUnique = true)]
    public class PrecioHistorico : IEntity, IValidate
    {
        public int Id { get; set; }
        [Required]
        public int ProductoId { get; set; }
        
        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; } 
        [Required]
        public DateOnly Fecha {  get; set; }
        [Required]
        public decimal Precio { get; set; }

        public PrecioHistorico(int productoId, decimal precio) {
            ProductoId = productoId;
            Precio = precio;
            Fecha = DateOnly.FromDateTime(DateTime.UtcNow);
            Validate();
        }

        internal PrecioHistorico() { }

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
    }
}
