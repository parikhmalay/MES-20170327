using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.RFQType
{
    public interface IRFQTypeRepository : ICrudMethods<MES.DTO.Library.Setup.RFQType.RFQType, int?, string,
        MES.DTO.Library.Setup.RFQType.RFQType, int, bool?, int, MES.DTO.Library.Setup.RFQType.RFQType>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.RFQType.RFQType>> GetRFQTypes(NPE.Core.IPage<MES.DTO.Library.Setup.RFQType.SearchCriteria> paging);
    }
}
