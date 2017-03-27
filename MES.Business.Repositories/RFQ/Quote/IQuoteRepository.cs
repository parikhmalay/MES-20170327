using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Quote
{
    public interface IQuoteRepository : ICrudMethods<MES.DTO.Library.RFQ.Quote.Quote, string, string,
           MES.DTO.Library.RFQ.Quote.Quote, string, bool?, string, MES.DTO.Library.RFQ.Quote.Quote>
    {
        ITypedResponse<List<MES.DTO.Library.RFQ.Quote.Quote>> GetQuoteList(NPE.Core.IPage<MES.DTO.Library.RFQ.Quote.SearchCriteria> paging);
        NPE.Core.ITypedResponse<string> InsertQuote(DTO.Library.RFQ.Quote.Quote quoteDetails);
        NPE.Core.ITypedResponse<string> UpdateQuote(DTO.Library.RFQ.Quote.Quote quoteDetails);
        ITypedResponse<bool?> exportToExcel(MES.DTO.Library.RFQ.Quote.Quote quote);

        ITypedResponse<List<MES.DTO.Library.RFQ.Quote.Quote>> GetQuotesToCustomer(string rfqId);
        NPE.Core.ITypedResponse<string> SaveQuoteCalculationHistory(DTO.Library.RFQ.Quote.Quote quoteData);
    }
}
