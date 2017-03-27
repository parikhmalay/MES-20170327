using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.Remarks
{
    public interface IRemarksRepository : ICrudMethods<MES.DTO.Library.Setup.Remarks.Remarks, int?, string,
        MES.DTO.Library.Setup.Remarks.Remarks, int, bool?, int, MES.DTO.Library.Setup.Remarks.Remarks>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Remarks.Remarks>> GetRemarks(NPE.Core.IPage<MES.DTO.Library.Setup.Remarks.SearchCriteria> paging);
    }
}
