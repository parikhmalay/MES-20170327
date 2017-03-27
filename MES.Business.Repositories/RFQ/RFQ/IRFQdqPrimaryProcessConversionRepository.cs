using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQdqPrimaryProcessConversionRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion>> GetRFQdqPrimaryProcessConversionList(int RFQSupplierPartDQId);
    }
}
