using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.ChangeRequest
{
    public interface IDocumentRepository : ICrudMethods<MES.DTO.Library.APQP.ChangeRequest.Document, int?, string,
          MES.DTO.Library.APQP.ChangeRequest.Document, int, bool?, int, MES.DTO.Library.APQP.ChangeRequest.Document>
    {
        List<MES.DTO.Library.APQP.ChangeRequest.Document> GetDocumentList(int changeRequestId);
    }
}
