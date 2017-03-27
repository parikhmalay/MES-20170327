using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class APQPNewBusinessAwardedReport
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> ProjectKickoffDate { get; set; }
        public string PartNumber { get; set; }
        public string RFQNumber { get; set; }
        public string Customer { get; set; }
        public string MonthOfPoLaunch { get; set; }
        public string SalesAccountManager { get; set; }
        public string ToolingSales { get; set; }
        public string ToolingCosts { get; set; }
        public string Quantity { get; set; }
        public string SellingPrice { get; set; }
        public string AnnualSales { get; set; }
        public string PurchasePrice { get; set; }
        public string ProductMargin { get; set; }
        public string AnnualCOGS { get; set; }
        public string SupplierName { get; set; }
        public string ManufacturerName { get; set; }
        public string ShippingCost { get; set; }
        public string TotalShippingCost { get; set; }
        public string COGSSupplier_Shipping { get; set; }
        public string ToolingMargin { get; set; }
        public string GrossMarginAfterShipping { get; set; }
        public string GrossMarginAfterShippingUSD { get; set; }
        public string InventoryRanking { get; set; }
        public string MESWarehouse { get; set; }
    }
    public class APQPNewBusinessAwardedReportSearchCriteria
    {
        public Nullable<System.DateTime> WeeklyDateFrom { get; set; }
        public Nullable<System.DateTime> WeeklyDateTo { get; set; }
        public Nullable<double> ShippingCost { get; set; }
    }
}
