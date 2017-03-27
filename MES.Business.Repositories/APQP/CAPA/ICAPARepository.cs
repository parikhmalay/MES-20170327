using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.CAPA
{
    public interface ICAPARepository : ICrudMethods<MES.DTO.Library.APQP.CAPA.CAPA, int?, string,
          MES.DTO.Library.APQP.CAPA.CAPA, int, bool?, int, MES.DTO.Library.APQP.CAPA.CAPA>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.CAPA.CAPA>> GetCAPAList(NPE.Core.IPage<MES.DTO.Library.APQP.CAPA.SearchCriteria> searchCriteria);
        ITypedResponse<List<MES.DTO.Library.Common.LookupFields>> getSAPPartsList(string supplierName, string customerName);
        ITypedResponse<string> CheckPartAssociationWithSupplier(DTO.Library.APQP.CAPA.CAPA capa);
        ITypedResponse<bool?> GenerateCAPAForm(DTO.Library.APQP.CAPA.CAPA capa);
        ITypedResponse<bool?> DeleteCAPAPartAffectedDetail(int CAPAPartAffectedDetailId);
        ITypedResponse<List<MES.DTO.Library.APQP.CAPA.capaPartDocument>> GetPartDocumentList(int cAPAPartAffectedDetailId, string SectionName);
        ITypedResponse<int?> SavePartDocument(DTO.Library.APQP.CAPA.capaPartDocument document);
        ITypedResponse<bool?> DeletePartDocument(int documentId);
        ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData emailData);
    }
}
