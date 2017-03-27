using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Supplier
{
    public interface ISuppliersRepository : ICrudMethods<MES.DTO.Library.RFQ.Supplier.Suppliers, int?, string,
          MES.DTO.Library.RFQ.Supplier.Suppliers, int, bool?, int, MES.DTO.Library.RFQ.Supplier.Suppliers>
    {
        ITypedResponse<List<MES.DTO.Library.RFQ.Supplier.Suppliers>> GetSuppliersList(NPE.Core.IPage<MES.DTO.Library.RFQ.Supplier.SearchCriteria> paging);
        ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData emailData);
        NPE.Core.ITypedResponse<bool?> DeleteMultiple(string SupplierIds);
        NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.Suppliers> FindByCode(string supplierCode);
    }
}
