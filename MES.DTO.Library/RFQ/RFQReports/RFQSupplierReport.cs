using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region RFQ Supplier list report
    public class RFQSupplierReport
    {
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Website { get; set; }
        public string CompanyPhone1 { get; set; }
        public string CompanyPhone2 { get; set; }
        public string CompanyFax { get; set; }
        public string Comments { get; set; }
        public string PrimaryContact { get; set; }
        public string Email { get; set; }
        public string DirectPhone { get; set; }
    }
    public class RFQSupplierReportSearch
    {
        public string SQIds { get; set; }
        public string CountryIds { get; set; }
        public int ReportId { get; set; }
    }
    #endregion
}
