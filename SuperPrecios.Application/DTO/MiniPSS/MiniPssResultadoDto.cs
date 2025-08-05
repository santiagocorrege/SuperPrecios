using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class MiniPssResultadoDto
    {
        [JsonPropertyName("resultados")]
        public List<ResultadoDto> Resultados { get; set; } = new();

        [JsonPropertyName("total_productos")]
        public int TotalProductos { get; set; }

        [JsonPropertyName("productos_matched")]
        public int ProductosMatched { get; set; }

        [JsonPropertyName("productos_nuevos")]
        public int ProductosNuevos { get; set; }

        [JsonPropertyName("total_marcas")]
        public int TotalMarcas { get; set; }

        [JsonPropertyName("marcas_matched")]
        public int MarcasMatched { get; set; }

        [JsonPropertyName("marcas_nuevas")]
        public int MarcasNuevas { get; set; }

        [JsonPropertyName("tiempo_procesamiento")]
        public double TiempoProcesamiento { get; set; }
    }
}
