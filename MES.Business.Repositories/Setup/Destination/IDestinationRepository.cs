using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.Destination
{
    public interface IDestinationRepository : ICrudMethods<MES.DTO.Library.Setup.Destination.Destination, int?, string,
          MES.DTO.Library.Setup.Destination.Destination, int, bool?, int, MES.DTO.Library.Setup.Destination.Destination>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Destination.Destination>> GetDestinationsList(NPE.Core.IPage<MES.DTO.Library.Setup.Destination.SearchCriteria> paging);
    }
}
