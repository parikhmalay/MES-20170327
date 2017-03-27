using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region RFQ Supplier Activity Level Report
    public class RFQSupplierActivityLevelReport
    {
        public string Supplier { get; set; }
        public System.DateTime SystemDate { get; set; }
        public string SystemDateString { get; set; }
        public string SentAnyRFQ { get; set; }
        public Nullable<int> NoOfRFQSent { get; set; }
        public string QuotedAnyProject { get; set; }
        public Nullable<int> NoOfRFQsQuoted { get; set; }
        public string LastRFQQuoted { get; set; }
        public Nullable<System.DateTime> DateLastQuoted { get; set; }
        public string DateLastQuotedString { get; set; }
    }
    public class RFQSupplierActivityLevelReportSearch
    {
        public string SupplierIds { get; set; }
        public string SQIds { get; set; }
        public string CountryIds { get; set; }
        public string CommodityIds { get; set; }
        public int ReportId { get; set; }
    }
    #endregion
}
