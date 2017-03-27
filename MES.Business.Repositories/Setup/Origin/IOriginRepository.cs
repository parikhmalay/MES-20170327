using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.Origin
{
    public interface IOriginRepository : ICrudMethods<MES.DTO.Library.Setup.Origin.Origin, int?, string,
        MES.DTO.Library.Setup.Origin.Origin, int, bool?, int, MES.DTO.Library.Setup.Origin.Origin>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Origin.Origin>> GetOrigins(NPE.Core.IPage<MES.DTO.Library.Setup.Origin.SearchCriteria> paging);
    }
}
