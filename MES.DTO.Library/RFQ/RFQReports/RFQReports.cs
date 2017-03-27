using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    public class RFQReports
    {

    }

    #region RFQ Analysis and Business(Alina) report with chart
    /// <summary>
    /// to hold the data of RFQAnalysisReportFirstPart SP
    /// </summary>
    public class RFQAnalysisReport
    {
        public string RfqDate { get; set; }
        public Nullable<int> NoOfRfq { get; set; }
        public Nullable<int> NewRFQ { get; set; }
        public Nullable<int> NewCustomer { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<decimal> RFQDateTotalQuoted { get; set; }
        public Nullable<decimal> RFQDateTotalToolingCost { get; set; }
        public List<RFQAnalysisReportDetails> lstRFQAnalysisReportDetails = new List<RFQAnalysisReportDetails>();
        public List<RFQAnalysisChart> lstRFQAnalysisChart = new List<RFQAnalysisChart>();

    }
    /// <summary>
    /// to hold the data of RFQAnalysisReportSecondPart SP
    /// </summary>
    public class RFQAnalysisReportDetails
    {
        public string RFQ { get; set; }
        public string RfqDate { get; set; }
        public string QuoteDate { get; set; }
        public string Customer { get; set; }
        public string AccountManager { get; set; }
        public Nullable<decimal> totalquoted { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public string Commodity { get; set; }
        public string Process { get; set; }
        public int sortorder { get; set; }
        public string RFQSource { get; set; }
        public string RFQType { get; set; }
        public string IndustryType { get; set; }
        public string GroupByValue { get; set; }

    }
    /// <summary>
    /// to hold the data of RFQAnalysisChart SP
    /// </summary>
    public class RFQAnalysisChart
    {
        public Nullable<decimal> NoOfRfq { get; set; }
        public string DisplayName { get; set; }
    }
    public class RFQAnalysisReportSearch
    {
        public Nullable<DateTime> RFQDateFrom { get; set; }
        public Nullable<DateTime> RFQDateTo { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public string SAMIds { get; set; }
        public string CustomerIds { get; set; }
        public string CommodityIds { get; set; }
        public string RFQSourceIds { get; set; }
        public string RFQTypeIds { get; set; }
        public string IndustryTypeIds { get; set; }
        public string GroupBy { get; set; }
        public int ReportId { get; set; }
    }
    #endregion

    #region RFQ "non award reason report" with chart
    public class RFQNonAwardReasonReport
    {
        public string RfqDate { get; set; }
        public Nullable<int> NoOfRfq { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<decimal> RFQDateTotalQuoted { get; set; }
        public Nullable<decimal> RFQDateTotalToolingCost { get; set; }
        public List<RFQNonAwardReasonReportDetails> lstRFQNonAwardReasonReportDetails = new List<RFQNonAwardReasonReportDetails>();
        public List<RFQNonAwardReasonChart> lstRFQNonAwardReasonChart = new List<RFQNonAwardReasonChart>();
    }
    public class RFQNonAwardReasonReportDetails
    {
        public string NonAwardFeedback { get; set; }
        public string NonAwardDetailedFeedback { get; set; }
        public string RFQ { get; set; }
        public string RfqDate { get; set; }
        public string Customer { get; set; }
        public string AccountManager { get; set; }
        public Nullable<decimal> totalquoted { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public string Commodity { get; set; }
        public string Process { get; set; }
        public int sortorder { get; set; }
        public string RFQSource { get; set; }
        public string RFQType { get; set; }
        public string GroupByValue { get; set; }

    }
    public class RFQNonAwardReasonChart
    {
        public Nullable<decimal> NoOfRfq { get; set; }
        public string DisplayName { get; set; }
    }
    public class RFQNonAwardReasonReportSearch
    {
        public Nullable<DateTime> RFQDateFrom { get; set; }
        public Nullable<DateTime> RFQDateTo { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public string SAMIds { get; set; }
        public string CustomerIds { get; set; }
        public string CommodityIds { get; set; }
        public string RFQSourceIds { get; set; }
        public string GroupBy { get; set; }
        public int ReportId { get; set; }
    }
    #endregion

}
