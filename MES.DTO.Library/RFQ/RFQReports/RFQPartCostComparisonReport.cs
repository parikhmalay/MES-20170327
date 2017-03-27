using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQReports
{
    public class RFQPartCostComparisonReport
    {
        public string RFQNo { get; set; }
        public System.DateTime rfqDate { get; set; }
        public string rfqDateString { get; set; }
        public string Customer { get; set; }
        public List<RFQPartCostComparisonReportThirdPart> lstRFQPartCostComparisonReportThirdPart { get; set; }
    }
  
    public class RFQPartCostComparisonReportThirdPart
    {
        public Nullable<DateTime> UpdatedDate { get; set; }
        public decimal PiecePrice { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public decimal SupplierCostPerKg { get; set; }
        public int SupplierId { get; set; }
        public int rfqPartId { get; set; }
    }

    public class RFQPartCostComparisonReportSearch
    {
        public string RFQIds { get; set; }
        public int ReportId { get; set; }
    }
}
