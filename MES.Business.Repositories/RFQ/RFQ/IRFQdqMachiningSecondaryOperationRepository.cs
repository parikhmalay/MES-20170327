using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQdqMachiningSecondaryOperationRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation>> GetRFQdqMachiningSecondaryOperationList(int RFQSupplierPartDQId);
    }
}
