using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RFQ.Quote
{
    public interface IQuoteCalculationHistoryRepository : ICrudMethods<MES.DTO.Library.RFQ.Quote.QuoteCalculationHistory, int?, string,
            MES.DTO.Library.RFQ.Quote.QuoteCalculationHistory, int, bool?, int, MES.DTO.Library.RFQ.Quote.QuoteCalculationHistory>
    {
        List<MES.DTO.Library.RFQ.Quote.QuoteCalculationHistory> GetQuoteCalculationHistory(string QuoteId);
    }
}
