using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Supplier
{
    public interface ISupplierAssessmentRepository: ICrudMethods<MES.DTO.Library.RFQ.Supplier.SupplierAssessment, int?, string,
          MES.DTO.Library.RFQ.Supplier.SupplierAssessment, int, bool?, int, MES.DTO.Library.RFQ.Supplier.SupplierAssessment>
    {
        List<MES.DTO.Library.RFQ.Supplier.AssessmentListDetail> GetSupplierAssessmentList(int suppllierId);

        ITypedResponse<DTO.Library.RFQ.Supplier.SupplierAssessment> GetAssessmentDetail(int supplierId);

        ITypedResponse<int?> CreateRevision(int assessmentId);
    } 
}
