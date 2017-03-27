using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.Forwarder
{
    public interface IForwarderRepository : ICrudMethods<MES.DTO.Library.Setup.Forwarder.Forwarder, int?, string,
          MES.DTO.Library.Setup.Forwarder.Forwarder, int, bool?, int, MES.DTO.Library.Setup.Forwarder.Forwarder>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Forwarder.Forwarder>> GetForwarderList(NPE.Core.IPage<MES.DTO.Library.Setup.Forwarder.SearchCriteria> paging);
    }
}
