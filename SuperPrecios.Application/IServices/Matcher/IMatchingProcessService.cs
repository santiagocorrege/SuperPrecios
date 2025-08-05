using SuperPrecios.Application.DTO.MiniPSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Application.IServices.Matcher
{
    public interface IMatchingProcessService
    {
        Task ProcessMatchingResultsAsync(MiniPssResultadoDto matchingResults);
    }
}
