using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.Process
{
    public interface IProcessRepository : ICrudMethods<MES.DTO.Library.Setup.Process.Process, int?, string,
        MES.DTO.Library.Setup.Process.Process, int, bool?, int, MES.DTO.Library.Setup.Process.Process>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Process.Process>> GetProcesses(NPE.Core.IPage<MES.DTO.Library.Setup.Process.SearchCriteria> paging);
    }
}
