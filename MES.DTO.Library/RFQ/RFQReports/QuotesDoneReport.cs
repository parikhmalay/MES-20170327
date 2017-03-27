using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region  Quotes done Report
    public class QuotesDoneReport
    {
        public string QuoteNumber { get; set; }
        public string RFQ { get; set; }
        public string Customer { get; set; }
        public string AccountManager { get; set; }
        public string Commodity { get; set; }
        public string Process { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.DateTime> QuoteDate { get; set; }
        public string QuoteResult { get; set; }
        public decimal AmountWon { get; set; }
        public List<QuotesDoneChart> lstQuotesDoneChart { get; set; }
        public string GroupByValue { get; set; }
        
    }
    public class QuotesDoneChart
    {
        public string DisplayName { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
    public class QuotesDoneReportSearch
    {
        public Nullable<DateTime> QuoteDateFrom { get; set; }
        public Nullable<DateTime> QuoteDateTo { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public string SAMIds { get; set; }
        public string CustomerIds { get; set; }
        public string GroupBy { get; set; }
        public int ReportId { get; set; }
    }
    #endregion

}
