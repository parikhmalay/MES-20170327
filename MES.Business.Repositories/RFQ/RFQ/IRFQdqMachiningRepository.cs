using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQdqMachiningRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQdqMachining, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQdqMachining, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQdqMachining>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachining>> GetRFQdqMachiningList(int RFQSupplierPartDQId);
    }
}
