using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Marca
{
    public class DtoMarcaAdd
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } // ✅ NUEVO: ID de Python

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
