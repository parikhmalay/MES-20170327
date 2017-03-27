using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Customer
{
    public interface IContactsRepository : ICrudMethods<MES.DTO.Library.RFQ.Customer.Contacts, int?, string,
          MES.DTO.Library.RFQ.Customer.Contacts, int, bool?, int, MES.DTO.Library.RFQ.Customer.Contacts>
    {
        List<MES.DTO.Library.RFQ.Customer.Contacts> GetContactsList(int customerId);
        NPE.Core.ITypedResponse<bool?> DeleteMultiple(string ContactIds);
        
    }
}
