using SuperPrecios.Application.DTO.Categoria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CategoriaCore = SuperPrecios.Domain.Entities.Categoria;

namespace SuperPrecios.Application.IServices.Categoria
{
    public interface ICategoriaAddService
    {
        public Task Run(DtoCategoriaAdd dto);
    }
}
