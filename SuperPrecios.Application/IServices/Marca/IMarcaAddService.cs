using SuperPrecios.Application.DTO.Marca;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Marca
{
    public interface IMarcaAddService
    {
        public Task AddAsync(DtoMarcaAdd dto);        
    }
}
