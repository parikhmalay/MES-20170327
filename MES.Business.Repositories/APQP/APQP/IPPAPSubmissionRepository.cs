using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.APQP
{
    public interface IPPAPSubmissionRepository : ICrudMethods<MES.DTO.Library.APQP.APQP.PPAPSubmission, int?, string,
         MES.DTO.Library.APQP.APQP.PPAPSubmission, int, bool?, int, MES.DTO.Library.APQP.APQP.PPAPSubmission>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.PPAPSubmission>> GetPPAPSubmissionList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
    }
}
