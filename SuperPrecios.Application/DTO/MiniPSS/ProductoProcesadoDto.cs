using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class ProductoProcesadoDto
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("marca")]
        public string? Marca { get; set; }

        [JsonPropertyName("marca_id")]
        public int? MarcaId { get; set; }

        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }

        [JsonPropertyName("imagen")]
        public string? Imagen { get; set; }

        [JsonPropertyName("divisa")]
        public string? Divisa { get; set; }

        [JsonPropertyName("matched")]
        public bool Matched { get; set; }

        [JsonPropertyName("producto_id")]
        public int? ProductoId { get; set; }
    }
}
