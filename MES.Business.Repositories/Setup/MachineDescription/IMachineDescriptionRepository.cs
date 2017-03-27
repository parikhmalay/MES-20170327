using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.MachineDescription
{
    public interface IMachineDescriptionRepository : ICrudMethods<MES.DTO.Library.Setup.MachineDescription.MachineDescription, int?, string,
          MES.DTO.Library.Setup.MachineDescription.MachineDescription, int, bool?, int, MES.DTO.Library.Setup.MachineDescription.MachineDescription>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.MachineDescription.MachineDescription>> GetMachineDescriptionList(NPE.Core.IPage<MES.DTO.Library.Setup.MachineDescription.SearchCriteria> paging);
    }
}
