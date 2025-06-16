using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductoCore = SuperPrecios.Domain.Entities.Producto;

namespace SuperPrecios.Domain.DTORepository;

public class DtoRepositorioProductoPaginado
{
    public int PaginaActual { get; set; }

    public int TotalPaginas { get; set; }
    public IEnumerable<ProductoCore> Productos { get; set; }
}
