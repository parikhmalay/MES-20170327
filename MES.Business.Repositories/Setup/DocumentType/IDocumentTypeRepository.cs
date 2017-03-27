using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Setup.DocumentType
{
    public interface IDocumentTypeRepository : ICrudMethods<MES.DTO.Library.Setup.DocumentType.DocumentType, int?, string,
          MES.DTO.Library.Setup.DocumentType.DocumentType, int, bool?, int, MES.DTO.Library.Setup.DocumentType.DocumentType>
    {
        ITypedResponse<System.Collections.Generic.List<MES.DTO.Library.Setup.DocumentType.DocumentType>> GetDocumentTypesList(NPE.Core.IPage<MES.DTO.Library.Setup.DocumentType.SearchCriteria> paging);
    }
}
