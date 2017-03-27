using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.ShipmentTracking
{
    public interface IShipmentsRepository : ICrudMethods<MES.DTO.Library.ShipmentTracking.Shipments, int?, string,
          MES.DTO.Library.ShipmentTracking.Shipments, int, bool?, int, MES.DTO.Library.ShipmentTracking.Shipments>
    {
        ITypedResponse<List<MES.DTO.Library.ShipmentTracking.Shipments>> GetShipmentList(NPE.Core.IPage<MES.DTO.Library.ShipmentTracking.SearchCriteria> paging);
        ITypedResponse<bool?> DownloadShipment();
        ITypedResponse<int?> UploadShipment(string uploadShipmentFilePath);
        ITypedResponse<bool?> exportToExcelShipmentReport(NPE.Core.IPage<MES.DTO.Library.ShipmentTracking.SearchCriteria> paging);
    }
}
