using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.CommodityType
{
    public interface ICommodityTypeRepository : ICrudMethods<MES.DTO.Library.Setup.CommodityType.CommodityType, int?, string,
          MES.DTO.Library.Setup.CommodityType.CommodityType, int, bool?, int, MES.DTO.Library.Setup.CommodityType.CommodityType>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.CommodityType.CommodityType>> GetCommodityTypesList(NPE.Core.IPage<MES.DTO.Library.Setup.CommodityType.SearchCriteria> paging);
    }
}
