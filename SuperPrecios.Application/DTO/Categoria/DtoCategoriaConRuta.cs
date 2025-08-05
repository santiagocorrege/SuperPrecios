using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.Categoria
{
    public class DtoCategoriaConRuta
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? PadreId { get; set; }
        public string RutaCompleta { get; set; }
    }
}
