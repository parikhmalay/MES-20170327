using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.RFQPriority
{
    public interface IRFQPriorityRepository : ICrudMethods<MES.DTO.Library.Setup.RFQPriority.RFQPriority, int?, string,
        MES.DTO.Library.Setup.RFQPriority.RFQPriority, int, bool?, int, MES.DTO.Library.Setup.RFQPriority.RFQPriority>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.RFQPriority.RFQPriority>> GetRFQPrioritys(NPE.Core.IPage<MES.DTO.Library.Setup.RFQPriority.SearchCriteria> paging);
    }
}
