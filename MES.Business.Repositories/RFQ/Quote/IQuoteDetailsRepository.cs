using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Quote
{
    public interface IQuoteDetailsRepository : ICrudMethods<MES.DTO.Library.RFQ.Quote.QuoteDetails, string, string,
            MES.DTO.Library.RFQ.Quote.QuoteDetails, string, bool?, string, MES.DTO.Library.RFQ.Quote.QuoteDetails>
    {
        ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> GetPartsToQuote(string rfqId);
        ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> GetQuoteDetails(string quoteId, bool isR);
        ITypedResponse<MES.DTO.Library.RFQ.Quote.QuoteDetails> GetSupplierQuotedDetails(MES.DTO.Library.RFQ.Quote.QuoteDetails qdItem);

        ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> GetQuotePartsDetail(MES.DTO.Library.RFQ.Quote.Quote qItem);
        ITypedResponse<List<DTO.Library.RFQ.Quote.QuoteDetails>> FindByQuoteId(string quoteId);
        ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> GetQuotesGeneralDetails(string quoteId, bool isR);
    }
}
