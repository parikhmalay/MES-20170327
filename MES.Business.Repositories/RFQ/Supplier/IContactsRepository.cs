using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Supplier
{
    public interface IContactsRepository : ICrudMethods<MES.DTO.Library.RFQ.Supplier.Contacts, int?, string,
          MES.DTO.Library.RFQ.Supplier.Contacts, int, bool?, int, MES.DTO.Library.RFQ.Supplier.Contacts>
    {
        List<MES.DTO.Library.RFQ.Supplier.Contacts> GetContactsList(int supplierId);
        NPE.Core.ITypedResponse<bool?> DeleteMultiple(string ContactIds);
        
    }
}
