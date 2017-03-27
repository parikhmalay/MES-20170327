using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.DefectTracking
{
    public interface IDefectTrackingDetailRepository : ICrudMethods<MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail, int?, string,
          MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail, int, bool?, int, MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail>> GetDefectTrackingDetailList(int defectTrackingId);        
    }
}
