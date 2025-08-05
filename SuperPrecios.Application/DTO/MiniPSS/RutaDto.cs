using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class RutaDto
    {
        [JsonPropertyName("ruta")]
        public string Ruta { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("rutaCompleta")]
        public string RutaCompleta { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("matched")]
        public bool Matched { get; set; }
    }
}
