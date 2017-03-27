using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.ProjectStage
{
    public interface IProjectStageRepository : ICrudMethods<MES.DTO.Library.Setup.ProjectStage.ProjectStage, int?, string,
          MES.DTO.Library.Setup.ProjectStage.ProjectStage, int, bool?, int, MES.DTO.Library.Setup.ProjectStage.ProjectStage>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.ProjectStage.ProjectStage>> GetProjectStagesList(NPE.Core.IPage<MES.DTO.Library.Setup.ProjectStage.SearchCriteria> paging);
    }
}
