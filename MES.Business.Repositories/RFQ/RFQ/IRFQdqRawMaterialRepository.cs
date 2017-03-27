using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQdqRawMaterialRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQdqRawMaterial, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQdqRawMaterial, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQdqRawMaterial>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqRawMaterial>> GetRFQdqRawMaterialList(int RFQSupplierPartDQId);
    }
}
