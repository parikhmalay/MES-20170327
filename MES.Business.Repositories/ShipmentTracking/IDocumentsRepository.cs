using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.ShipmentTracking
{
    public interface IDocumentsRepository : ICrudMethods<MES.DTO.Library.ShipmentTracking.Documents, int?, string,
          MES.DTO.Library.ShipmentTracking.Documents, int, bool?, int, MES.DTO.Library.ShipmentTracking.Documents>
    {
        List<MES.DTO.Library.ShipmentTracking.Documents> GetDocumentsList(int shipmentId);
    }
}
