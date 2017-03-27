
using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.RFQSource
{
    public interface IRFQSourceRepository : ICrudMethods<MES.DTO.Library.Setup.RFQSource.RFQSource, int?, string,
        MES.DTO.Library.Setup.RFQSource.RFQSource, int, bool?, int, MES.DTO.Library.Setup.RFQSource.RFQSource>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.RFQSource.RFQSource>> GetRFQSources(NPE.Core.IPage<MES.DTO.Library.Setup.RFQSource.SearchCriteria> paging);
    }
}
