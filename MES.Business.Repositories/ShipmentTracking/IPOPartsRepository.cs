using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.ShipmentTracking
{
    public interface IPOPartsRepository : ICrudMethods<MES.DTO.Library.ShipmentTracking.POParts, int?, string,
          MES.DTO.Library.ShipmentTracking.POParts, int, bool?, int, MES.DTO.Library.ShipmentTracking.POParts>
    {
        List<MES.DTO.Library.ShipmentTracking.POParts> GetPOPartsList(int shipmentId);
    }
}
