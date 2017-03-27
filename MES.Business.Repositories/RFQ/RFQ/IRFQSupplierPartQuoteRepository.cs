using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.RFQ
{
    public interface IRFQSupplierPartQuoteRepository : ICrudMethods<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuote, int?, string,
           MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuote, int, bool?, int, MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>
    {
        ITypedResponse<bool?> SaveSubmitQuote(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList);
        ITypedResponse<bool?> SaveSubmitNoQuote(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetRFQSupplierPartQuoteList(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<bool?> SaveSubmitDQ(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList);
        ITypedResponse<bool?> SaveSubmitNoDQ(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetDQRFQSupplierPartQuoteList(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        //Supplier Quote
        ITypedResponse<bool?> SaveSupplierQuoteList(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList);
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetSupplierQuoteList(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<bool?> exportToExcelSupplierQuote(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQ> getRFQDetails(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<bool?> DownloadRfqSupplierPartQuote(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> UploadRfqSupplierPartQuote(NPE.Core.IPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging);
        ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQ> GetRFQPartCostComparisonList(string rfqId);

    }
}
