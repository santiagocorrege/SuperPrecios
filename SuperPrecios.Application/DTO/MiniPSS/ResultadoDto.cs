using SuperPrecios.Application.DTO.Supermercado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class ResultadoDto
    {
        [JsonPropertyName("supermercado")]
        public DtoSupermercadoGet Supermercado { get; set; } = new();

        [JsonPropertyName("ruta")]
        public RutaDto Ruta { get; set; } = new();

        [JsonPropertyName("productos_procesados")]
        public List<ProductoProcesadoDto> ProductosProcesados { get; set; } = new();

        [JsonPropertyName("marcas_procesadas")]
        public List<MarcaProcesadaDto> MarcasProcesadas { get; set; } = new();

        [JsonPropertyName("estadisticas")]
        public Dictionary<string, int> Estadisticas { get; set; } = new();
    }
}
