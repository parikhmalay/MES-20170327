using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.Designation
{
    public interface IDesignationRepository : ICrudMethods<MES.DTO.Library.Setup.Designation.Designation, int?, string,
          MES.DTO.Library.Setup.Designation.Designation, int, bool?, int, MES.DTO.Library.Setup.Designation.Designation>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.Designation.Designation>> GetDesignationList(NPE.Core.IPage<MES.DTO.Library.Setup.Designation.SearchCriteria> paging);
    }
}
