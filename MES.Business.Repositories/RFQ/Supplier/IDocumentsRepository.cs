using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Supplier
{
    public interface IDocumentsRepository : ICrudMethods<MES.DTO.Library.RFQ.Supplier.Documents, int?, string,
          MES.DTO.Library.RFQ.Supplier.Documents, int, bool?, int, MES.DTO.Library.RFQ.Supplier.Documents>
    {
        List<MES.DTO.Library.RFQ.Supplier.Documents> GetDocumentsList(int supplierId);
    }
}
