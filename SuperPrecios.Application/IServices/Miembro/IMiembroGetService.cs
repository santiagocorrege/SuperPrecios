using SuperPrecios.Application.DTO.Miembro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Miembro
{
    public interface IMiembroGet
    {
        public Task<DtoMiembroGet> RunByEmail(string email);

        public Task<DtoMiembroGet> RunById(int id);

        public Task<DtoMiembroUpdate> RunGetUpdate(int id);

        public Task<IEnumerable<DtoMiembroGet>> RunByEmailList(string email);

        public Task<IEnumerable<DtoMiembroGet>> Run();
    }
}
