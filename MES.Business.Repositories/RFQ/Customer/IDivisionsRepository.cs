using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Customer
{
    public interface IDivisionsRepository : ICrudMethods<MES.DTO.Library.RFQ.Customer.Divisions, int?, string,
          MES.DTO.Library.RFQ.Customer.Divisions, int, bool?, int, MES.DTO.Library.RFQ.Customer.Divisions>
    {
        List<MES.DTO.Library.RFQ.Customer.Divisions> GetDivisionsList(int supplierId);
        NPE.Core.ITypedResponse<bool?> DeleteMultiple(string DivisionIds);
    }
}
