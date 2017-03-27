using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQ, string, string,
          MES.DTO.Library.RFQ.RFQ.RFQ, string, bool?, string, MES.DTO.Library.RFQ.RFQ.RFQ>
    {
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQ>> GetRFQList(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.SearchCriteria> paging);
        ITypedResponse<DTO.Library.RFQ.RFQ.RFQ> CreateRevision(string rfqNo);
        ITypedResponse<bool?> SendRFQCloseOutEmail(MES.DTO.Library.Common.EmailData emailData);
    }
}
