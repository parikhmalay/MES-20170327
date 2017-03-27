using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.TriggerPoint
{
    public interface ITriggerPointRepository : ICrudMethods<MES.DTO.Library.Setup.TriggerPoint.TriggerPoint, int?, string,
          MES.DTO.Library.Setup.TriggerPoint.TriggerPoint, int, bool?, int, MES.DTO.Library.Setup.TriggerPoint.TriggerPoint>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.TriggerPoint.TriggerPoint>> GetTriggerPointsList(NPE.Core.IPage<MES.DTO.Library.Setup.TriggerPoint.SearchCriteria> paging);
    }
}
