using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQdqSurfaceTreatmentRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment>> GetRFQdqSurfaceTreatmentList(int RFQSupplierPartDQId);
    }
}
