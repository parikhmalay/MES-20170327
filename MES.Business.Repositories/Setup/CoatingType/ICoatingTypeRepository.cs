using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.CoatingType
{
    public interface ICoatingTypeRepository : ICrudMethods<MES.DTO.Library.Setup.CoatingType.CoatingType, int?, string,
          MES.DTO.Library.Setup.CoatingType.CoatingType, int, bool?, int, MES.DTO.Library.Setup.CoatingType.CoatingType>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.CoatingType.CoatingType>> GetCoatingTypeList(NPE.Core.IPage<MES.DTO.Library.Setup.CoatingType.SearchCriteria> paging);
    }
}
