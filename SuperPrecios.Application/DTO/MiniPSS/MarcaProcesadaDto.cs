using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class MarcaProcesadaDto
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("marca_id")]
        public int MarcaId { get; set; }

        [JsonPropertyName("matched")]
        public bool Matched { get; set; }
    }
}
