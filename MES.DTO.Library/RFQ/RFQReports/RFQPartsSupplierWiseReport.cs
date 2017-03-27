using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    #region RFQ Quote Report by Supplier
    public class RFQPartsSupplierWiseReport
    {
        public string RFQNo { get; set; }
        public System.DateTime rfqDate { get; set; }
        public string rfqDateString { get; set; }
        public string Customer { get; set; }
        public List<RFQPartsSupplierWiseReportSecondPart> lstRFQPartsSupplierWiseReportSecondPart { get; set; }
    }
    public class RFQPartsSupplierWiseReportSecondPart
    {
        public string RFQNo { get; set; }
        public int RfqPartid { get; set; }
        public string CustomerPartNo { get; set; }
        public string PartDescription { get; set; }
        public string AdditionalPartDescription { get; set; }
        public Nullable<decimal> PartWeightKG { get; set; }
        public Nullable<int> EstimatedQty { get; set; }
        public Nullable<int> TotalParts { get; set; }
        public List<RFQPartsSupplierWiseReportThirdPart> lstRFQPartsSupplierWiseReportThirdPart { get; set; }
    }
    public class RFQPartsSupplierWiseReportThirdPart
    {
        public string RFQNo { get; set; }
        public int RfqPartId { get; set; }
        public string CustomerPartNo { get; set; }
        public Nullable<int> EstimatedQty { get; set; }
        public Nullable<decimal> PartWeightKG { get; set; }
        public string SupplierName { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ProcessCost { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public Nullable<decimal> OtherProcessCost { get; set; }
        public decimal UnitPrice { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string QuoteDateString { get; set; }
        public Nullable<decimal> SupplierCostPerKg { get; set; }
    }

    public class RFQPartsSupplierWiseReportSearch
    {
        public string RFQIds { get; set; }
        public int ReportId { get; set; }
    }
    #endregion

}
