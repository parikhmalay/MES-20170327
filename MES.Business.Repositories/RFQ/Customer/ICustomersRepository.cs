using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Customer
{
    public interface ICustomersRepository : ICrudMethods<MES.DTO.Library.RFQ.Customer.Customers, int?, string,
          MES.DTO.Library.RFQ.Customer.Customers, int, bool?, int, MES.DTO.Library.RFQ.Customer.Customers>
    {
        ITypedResponse<List<MES.DTO.Library.RFQ.Customer.Customers>> GetCustomersList(NPE.Core.IPage<MES.DTO.Library.RFQ.Customer.SearchCriteria> paging);
        NPE.Core.ITypedResponse<bool?> DeleteMultiple(string CustomerIds);
    }
}
