using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.DefectTracking
{
    public interface IPartDocumentRepository : ICrudMethods<MES.DTO.Library.APQP.DefectTracking.PartDocument, int?, string,
          MES.DTO.Library.APQP.DefectTracking.PartDocument, int, bool?, int, MES.DTO.Library.APQP.DefectTracking.PartDocument>
    {
        List<MES.DTO.Library.APQP.DefectTracking.PartDocument> GetPartDocumentList(int defectTrackingDetailId, int AssociatedToId);
    }
}
