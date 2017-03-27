using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.NonAwardFeedback
{
    public interface INonAwardFeedbackRepository : ICrudMethods<MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback, int?, string,
        MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback, int, bool?, int, MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>> GetNonAwardFeedbacks(NPE.Core.IPage<MES.DTO.Library.Setup.NonAwardFeedback.SearchCriteria> paging);
    }
}
