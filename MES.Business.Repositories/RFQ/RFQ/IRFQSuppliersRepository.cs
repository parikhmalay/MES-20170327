
using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQSuppliersRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQSuppliers, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQSuppliers, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQSuppliers>
    {
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> GetAvailableSuppliersList(NPE.Core.IPage<DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria> paging);
        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> GetRFQSuppliersList(NPE.Core.IPage<DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria> paging);

        NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQSuppliers> Save(DTO.Library.RFQ.RFQ.RFQSuppliers rFQSuppliers);

        NPE.Core.ITypedResponse<bool?> SendRFQToSuppliers(DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData);
        NPE.Core.ITypedResponse<bool?> ResendRFQToRfqSuppliers(DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData);

        ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData emailData);
        NPE.Core.ITypedResponse<bool?> DeleteRFQSuppliers(int RFQSupplierId);

        ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSuppliers>> GetQuotedSuppliers(string rfqId);

    }
}
