using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.SecondaryOperationDesc
{
    public interface ISecondaryOperationDescRepository : ICrudMethods<MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc, int?, string,
        MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc, int, bool?, int, MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>> GetSecondaryOperationDescs(NPE.Core.IPage<MES.DTO.Library.Setup.SecondaryOperationDesc.SearchCriteria> paging);
    }
}
