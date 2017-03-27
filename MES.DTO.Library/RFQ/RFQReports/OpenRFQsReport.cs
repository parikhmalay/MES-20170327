using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region open RFQ report
    public class OpenRFQsReport
    {
        public string RFQNumber { get; set; }
        public string Customer { get; set; }
        public System.DateTime RFQDate { get; set; }
        public string RFQDateString { get; set; }        
        public Nullable<System.DateTime> QuoteDueDate { get; set; }
        public string QuoteDueDateString { get; set; }
        public string SalesAccountManager { get; set; }
        public string ProjectName { get; set; }
        public string SupplierQuoted { get; set; }
        public Nullable<int> NumberOfPartNumbers { get; set; }
        public string Commodity { get; set; }
        public string Process { get; set; }
        public string QuoteFloated { get; set; }
        public string Rfqcoordinator { get; set; }
        public string RFQTypeName { get; set; }
        public string RFQPriority { get; set; }
        public string SupplierRequirement { get; set; }
    }
    public class OpenRFQsReportSearch
    {
        public Nullable<DateTime> RFQDateFrom { get; set; }
        public Nullable<DateTime> RFQDateTo { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public Nullable<DateTime> QuoteStartDate { get; set; }
        public Nullable<DateTime> QuoteEndDate { get; set; }
        public Nullable<DateTime> QuoteDateFrom { get; set; }
        public Nullable<DateTime> QuoteDateTo { get; set; }
        public string SAMIds { get; set; }
        public string CustomerIds { get; set; }
        public string ProjectName { get; set; }
        public string CountryIds { get; set; }
        public string SupQuoted { get; set; }
        public string RFQTypeIds { get; set; }
        public int ReportId { get; set; }
    }
    #endregion
}
