using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.DocumentManagement
{
    public interface IDocumentManagementRepository : ICrudMethods<MES.DTO.Library.APQP.DocumentManagement.DocumentManagement, int?, string,
          MES.DTO.Library.APQP.DocumentManagement.DocumentManagement, int, bool?, int, MES.DTO.Library.APQP.DocumentManagement.DocumentManagement>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.DocumentManagement.DocumentManagement>> GetDocumentManagementList(NPE.Core.IPage<MES.DTO.Library.APQP.DocumentManagement.SearchCriteria> searchCriteria);
        ITypedResponse<List<MES.DTO.Library.APQP.DocumentManagement.Document>> GetDocumentList(int DocumentManagementId);
        ITypedResponse<string> DownloadDocuments(List<string> DocumentFilePathList);
    }
}
