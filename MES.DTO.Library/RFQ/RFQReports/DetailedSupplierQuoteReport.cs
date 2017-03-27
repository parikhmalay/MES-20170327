using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region Detailed Supplier Quote report
    public class DetailedSupplierQuoteReport
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string SentAnyRFQ { get; set; }
        public Nullable<int> NoOfRFQSent { get; set; }
        public Nullable<int> NoOfRFQsQuoted { get; set; }
        public Nullable<int> NoOfParts { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ProcessCost { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public Nullable<decimal> OtherProcessCost { get; set; }
        public Nullable<decimal> TotalQuoted { get; set; }
        public Nullable<decimal> QuoteWon { get; set; }
    }
    public class DetailedSupplierQuoteSubReportFirstPart
    {
        public string RFQNo { get; set; }
        public List<DetailedSupplierQuoteSubReportSecondPart> lstDetailedSupplierQuoteSubReportSecondPart { get; set; }
    }
    public class DetailedSupplierQuoteSubReportSecondPart
    {
        public string RFQNo { get; set; }
        public int RfqPartId { get; set; }
        public string CustomerPartNo { get; set; }
        public string Description { get; set; }
        public string AddlDescription { get; set; }
        public Nullable<int> EstimatedQty { get; set; }
        public Nullable<decimal> PartWeightKG { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ProcessCost { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public Nullable<decimal> OtherProcessCost { get; set; }
        public decimal UnitPrice { get; set; }
        public string QuoteDateString { get; set; }
        public Nullable<decimal> SupplierCostPerKg { get; set; }
    }
    public class DetailedSupplierQuoteSearch
    {
        public string SupplierIds { get; set; }
        public int SupplierId { get; set; }
        public Nullable<DateTime> RfqSentDateFrom { get; set; }
        public Nullable<DateTime> RfqSentDateTo { get; set; }
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
        public int ReportId { get; set; }
        public bool isInnerReport { get; set; }
    }
    #endregion
}
