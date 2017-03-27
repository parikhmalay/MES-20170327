using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQdqOverheadRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQdqOverhead, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQdqOverhead, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQdqOverhead>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqOverhead>> GetRFQdqOverheadList(int RFQSupplierPartDQId);
    }
}
