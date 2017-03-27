using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region RFQs Quoted By Supplier
    public class RFQQuotedBySupplierReport
    {
        public string RFQID { get; set; }
        public string Customer { get; set; }
        public string ProjectName { get; set; }
        public System.DateTime RFQDate { get; set; }
        public string RFQDateString { get; set; }
        public string Quoted { get; set; }
        public string CustomerContactName { get; set; }
        public string Email { get; set; }
    }
    public class RFQQuotedBySupplierReportSearch
    {
        public string SupplierIds { get; set; }
        public string SQIds { get; set; }
        public Nullable<DateTime> RFQDateFrom { get; set; }
        public Nullable<DateTime> RFQDateTo { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public string CountryIds { get; set; }
        public int ReportId { get; set; }
    }
    #endregion
}
