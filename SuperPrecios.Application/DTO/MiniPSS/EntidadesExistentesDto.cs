using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.DTO.MiniPSS
{
    public class EntidadesExistentesDto
    {
        public HashSet<int> MarcasExistentes { get; set; } = new HashSet<int>();
        public HashSet<int> ProductosExistentes { get; set; } = new HashSet<int>();
    }

}
