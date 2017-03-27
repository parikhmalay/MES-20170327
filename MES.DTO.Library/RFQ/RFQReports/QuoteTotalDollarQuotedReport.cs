using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region Quote's Total Dollar Quoted Report
    public class QuoteTotalDollarQuotedReport
    {
        public Nullable<int> SortOrder { get; set; }
        public string Customer { get; set; }
        public string MonthYearName { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> TotalAnnualCost { get; set; }
        public Nullable<decimal> TotalQuoted { get; set; }
        public List<string> lstMonthName { get; set; }
    }

    public class QuoteTotalDollarQuotedReportSearch
    {
        public Nullable<DateTime> QuoteDateFrom { get; set; }
        public Nullable<DateTime> QuoteDateTo { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public int ReportId { get; set; }
    }
    #endregion

}
