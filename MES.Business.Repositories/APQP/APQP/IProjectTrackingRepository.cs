using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.APQP
{
    public interface IProjectTrackingRepository : ICrudMethods<MES.DTO.Library.APQP.APQP.ProjectTracking, int?, string,
          MES.DTO.Library.APQP.APQP.ProjectTracking, int, bool?, int, MES.DTO.Library.APQP.APQP.ProjectTracking>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.ProjectTracking>> GetProjectTrackingList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
    }
}
