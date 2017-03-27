using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region Supplier Parts Quote Report
    public class RFQSupplierPartsQuoteReport
    {
        public string RFQNo { get; set; }
        public System.DateTime rfqDate { get; set; }
        public string rfqDateString { get; set; }
        public string Customer { get; set; }
        public List<RFQSupplierPartsQuoteReportSecondPart> lstRFQSupplierPartsQuoteReportSecondPart { get; set; }
    }
    public class RFQSupplierPartsQuoteReportSecondPart
    {
        public string RFQNo { get; set; }
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> totalparts { get; set; }
        public Nullable<bool> NoQuote { get; set; }
        public List<RFQSupplierPartsQuoteReportThirdPart> lstRFQSupplierPartsQuoteReportThirdPart { get; set; }
    }
    public class RFQSupplierPartsQuoteReportThirdPart
    {
        public string RFQNo { get; set; }
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string CustomerPartNo { get; set; }
        public int RFQSupplirePartQuoteID { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ProcessCost { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public Nullable<decimal> OtherProcessCost { get; set; }
        public decimal UnitPrice { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> EstimatedQty { get; set; }
        public Nullable<decimal> PartWeightKG { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string QuoteDateString { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public string RawMaterialPriceAssumed { get; set; }
        public string QuoteAttachmentFilePath { get; set; }
        public Nullable<decimal> SupplierCostPerKg { get; set; }
    }
    public class RFQSupplierPartsQuoteReportSearch
    {
        public string RFQIds { get; set; }
        public int ReportId { get; set; }
    }
    #endregion
}
