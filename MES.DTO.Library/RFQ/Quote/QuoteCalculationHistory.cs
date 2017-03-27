using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.Quote
{
    public class QuoteCalculationHistory : CreateEditPropBase
    {
        public int Id { get; set; }
        public string QuoteId { get; set; }
        public int PartId { get; set; }
        public string CustomerPartNo { get; set; }
        public string PartDescription { get; set; }
        public string AdditionalPartDesc { get; set; }
        public Nullable<decimal> CustomDutiesPercent { get; set; }
        public Nullable<decimal> ShippingCostPercent { get; set; }
        public string ShippingCostCalMethod { get; set; }
        public Nullable<decimal> WarehousingPercent { get; set; }
        public Nullable<decimal> SGAProfitPercent { get; set; }
        public Nullable<decimal> SalesCommissionPercent { get; set; }
        public Nullable<decimal> ToolingCostPercent { get; set; }
        public Nullable<decimal> CustomDuties { get; set; }
        public Nullable<decimal> ShippingCost { get; set; }
        public Nullable<decimal> Warehousing { get; set; }
        public Nullable<decimal> SGAProfit { get; set; }
        public Nullable<decimal> SalesCommission { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public string UpdatedDateString { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
    }
}
