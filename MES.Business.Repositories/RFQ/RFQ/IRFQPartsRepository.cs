using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQPartsRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQParts, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQParts, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQParts>
    {
        List<MES.DTO.Library.RFQ.RFQ.RFQParts> GetRFQPartsList(string rfqId);
        NPE.Core.ITypedResponse<bool?> DeleteByRFQId(string rfqId);
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQParts>> UploadRFQParts(MES.DTO.Library.RFQ.RFQ.RFQ RFQ);
    }
}
