using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.ProjectCategory
{
    public interface IProjectCategoryRepository : ICrudMethods<MES.DTO.Library.Setup.ProjectCategory.ProjectCategory, int?, string,
          MES.DTO.Library.Setup.ProjectCategory.ProjectCategory, int, bool?, int, MES.DTO.Library.Setup.ProjectCategory.ProjectCategory>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.ProjectCategory.ProjectCategory>> GetProjectCategoryList(NPE.Core.IPage<MES.DTO.Library.Setup.ProjectCategory.SearchCriteria> paging);
    }
}
