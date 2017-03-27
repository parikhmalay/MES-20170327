using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.DefectTracking
{
    public interface IDefectTrackingRepository : ICrudMethods<MES.DTO.Library.APQP.DefectTracking.DefectTracking, int?, string,
          MES.DTO.Library.APQP.DefectTracking.DefectTracking, int, bool?, int, MES.DTO.Library.APQP.DefectTracking.DefectTracking>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTracking>> GetDefectTrackingList(NPE.Core.IPage<MES.DTO.Library.APQP.DefectTracking.SearchCriteria> searchCriteria);
        ITypedResponse<string> GetNewRMANumber();
        ITypedResponse<bool?> GenerateCAPAForm(DTO.Library.APQP.DefectTracking.DefectTracking defectTracking);
        ITypedResponse<List<MES.DTO.Library.APQP.DefectTracking.PartDocument>> GetPartDocumentList(int defectTrackingDetailId, string SectionName);
        ITypedResponse<int?> SavePartDocument(DTO.Library.APQP.DefectTracking.PartDocument document);
        ITypedResponse<bool?> DeletePartDocument(int documentId);
        ITypedResponse<bool?> DeleteDefectTrackingDetail(int DefectTrackingDetailId);
        ITypedResponse<bool?> GenerateRMAFormFromCAPA(int defectTrackingId);
    }
}
