using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Customer
{
    public interface IAddressRepository : ICrudMethods<MES.DTO.Library.RFQ.Customer.Address, int?, string,
          MES.DTO.Library.RFQ.Customer.Address, int, bool?, int, MES.DTO.Library.RFQ.Customer.Address>
    {
        List<MES.DTO.Library.RFQ.Customer.Address> GetAddressList(int customerId);
        NPE.Core.ITypedResponse<bool?> DeleteMultipleAddress(string AddressIds);        
    }
}
