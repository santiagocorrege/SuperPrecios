using Comun;
using Microsoft.EntityFrameworkCore;
using Prueba.Excepciones;
using Prueba.InterfacesEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prueba.Entidades
{    
    public class Producto : IEntity, IValidate
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
              
        public Marca Marca { get; set; }

        [ForeignKey(nameof(Marca))]
        public int MarcaId { get; set; }
        public Categoria Categoria { get; set; }

        [ForeignKey(nameof(CategoriaId))]
        public int CategoriaId {  get; set; }

        public Producto(string nombre, int localId, decimal precioInicial)
        {            
            Nombre = UtilidadesString.FormatearTexto(nombre);
            // Creamos el precio histórico inicial para este producto
            var precioHistorico = new PrecioHistorico
            {
                Fecha = DateOnly.FromDateTime(DateTime.UtcNow),
                Precio = precioInicial,
                Producto = this // Establecemos la relación
            };
            HistorialPrecio.Add(precioHistorico);
            PrecioActual = precioHistorico;
            LocalId = localId;
            Validate();
        }

        protected Producto() {            
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new ProductoException("Error: El nombre del producto no puede ser nulo");
            }
            if (LocalId == 0)
            {
                throw new ProductoException("Error: El local del producto no puede ser nulo");
            }
        }
    }
}
