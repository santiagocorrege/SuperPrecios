using SuperPrecios.Application.DTO.Categoria;
using SuperPrecios.Application.DTO.Marca;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Producto
{
    public class DtoProductoAdd
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } // ✅ NUEVO: ID de Python

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("marca_id")]
        public int MarcaId { get; set; }

        [JsonPropertyName("categoria_id")]
        public int CategoriaId { get; set; }

        [JsonPropertyName("imagen")]
        public string? ImgUrl { get; set; }
    }
}
