using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.DefectType
{
    public interface IDefectTypeRepository : ICrudMethods<MES.DTO.Library.Setup.DefectType.DefectType, int?, string,
          MES.DTO.Library.Setup.DefectType.DefectType, int, bool?, int, MES.DTO.Library.Setup.DefectType.DefectType>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.DefectType.DefectType>> GetDefectTypeList(NPE.Core.IPage<MES.DTO.Library.Setup.DefectType.SearchCriteria> paging);
    }
}
