using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.MachiningDescription
{
    public interface IMachiningDescriptionRepository : ICrudMethods<MES.DTO.Library.Setup.MachiningDescription.MachiningDescription, int?, string,
          MES.DTO.Library.Setup.MachiningDescription.MachiningDescription, int, bool?, int, MES.DTO.Library.Setup.MachiningDescription.MachiningDescription>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.MachiningDescription.MachiningDescription>> GetMachiningDescriptionList(NPE.Core.IPage<MES.DTO.Library.Setup.MachiningDescription.SearchCriteria> paging);
    }
}
