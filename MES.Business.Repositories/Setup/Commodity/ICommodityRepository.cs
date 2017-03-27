using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.Commodity
{
    public interface ICommodityRepository : ICrudMethods<MES.DTO.Library.Setup.Commodity.Commodity, int?, string,
          MES.DTO.Library.Setup.Commodity.Commodity, int, bool?, int, MES.DTO.Library.Setup.Commodity.Commodity>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Commodity.Commodity>> GetCommodityList(NPE.Core.IPage<MES.DTO.Library.Setup.Commodity.SearchCriteria> paging);
    }
}
