using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Dashboard
{
    public interface IDashboardRepository
    {
        ITypedResponse<MES.DTO.Library.Dashboard.Dashboard> GetDashboardSummary(NPE.Core.IPage<MES.DTO.Library.Dashboard.SearchCriteria> paging);
    }
}
