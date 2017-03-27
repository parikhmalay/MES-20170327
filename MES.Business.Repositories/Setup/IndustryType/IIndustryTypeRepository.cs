using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace MES.Business.Repositories.Setup.IndustryType
{
    public interface IIndustryTypeRepository : ICrudMethods<MES.DTO.Library.Setup.IndustryType.IndustryType, int?, string,
        MES.DTO.Library.Setup.IndustryType.IndustryType, int, bool?, int, MES.DTO.Library.Setup.IndustryType.IndustryType>
    {

        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.IndustryType.IndustryType>> GetIndustryTypes(NPE.Core.IPage<MES.DTO.Library.Setup.IndustryType.SearchCriteria> paging);
    }
}
