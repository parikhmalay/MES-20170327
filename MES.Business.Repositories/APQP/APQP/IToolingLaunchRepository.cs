using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.APQP
{
    public interface IToolingLaunchRepository : ICrudMethods<MES.DTO.Library.APQP.APQP.ToolingLaunch, int?, string,
         MES.DTO.Library.APQP.APQP.ToolingLaunch, int, bool?, int, MES.DTO.Library.APQP.APQP.ToolingLaunch>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.ToolingLaunch>> GetToolingLaunchList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
    }
}
