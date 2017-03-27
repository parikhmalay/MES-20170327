using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.Status
{
    public interface IStatusRepository : ICrudMethods<MES.DTO.Library.Setup.Status.Status, int?, string,
        MES.DTO.Library.Setup.Status.Status, int, bool?, int, MES.DTO.Library.Setup.Status.Status>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Status.Status>> GetStatus(NPE.Core.IPage<MES.DTO.Library.Setup.Status.SearchCriteria> paging);
    }
}
