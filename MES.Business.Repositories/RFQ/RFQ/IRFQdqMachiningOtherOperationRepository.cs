using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQdqMachiningOtherOperationRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation>> GetRFQdqMachiningOtherOperationList(int RFQSupplierPartDQId);
    }
}
