using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.Quote
{
    public class QuoteDetails : CreateEditPropBase
    {
        public int Id { get; set; }
        public string QuoteId { get; set; }
        public string QuoteRevision { get; set; }
        public int? SupplierId { get; set; }
        public int PartId { get; set; }

        #region "Part properties"
        public string CustomerPartNo { get; set; }
        public string RevLevel { get; set; }
        public string PartDescription { get; set; }
        public string AdditionalPartDesc { get; set; }
        public int? EstimatedQty { get; set; }
        public string MaterialType { get; set; }
        public decimal? PartWeightKG { get; set; }
        public string RfqId { get; set; }
        #endregion

        #region Percent properties
        public string CustomDutiesPercent { get; set; }
        public string ShippingCostPercent { get; set; }
        public string ShippingCostCalMethod { get; set; }
        public string WarehousingPercent { get; set; }
        public string SGAProfitPercent { get; set; }
        public string SalesCommissionPercent { get; set; }
        public string ToolingCostPercent { get; set; }
        #endregion

        public Nullable<decimal> SupplierToolingCost { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> ShippingCost { get; set; }
        public Nullable<decimal> Warehousing { get; set; }
        public Nullable<decimal> SGAProfit { get; set; }
        public Nullable<decimal> SalesCommission { get; set; }
        public Nullable<decimal> CustomDuties { get; set; }
        public Nullable<decimal> SupplierQuotedPrice { get; set; }

        public Nullable<int> MinOrderQty { get; set; }
        public string Leadtime { get; set; }
        public string ShippingAssumption { get; set; }

        public Nullable<decimal> SupplierPriceUsed { get; set; }
        public Nullable<decimal> FinalMESPrice { get; set; }

        public DateTime? QuoteDate { get; set; }

        public Nullable<decimal> TotalAnnualCost { get; set; }
        public Nullable<decimal> SupplierCostPerKg { get; set; }
        public Nullable<decimal> FinalMESPerKg { get; set; }

        public string SupplierToolingLeadtime { get; set; }
        public string ToolingWarranty { get; set; }

        public bool IsAPQPItem { get; set; }
        public bool chkSelect { get; set; }

    }
    #region Criteria
    public class QDSearchCriteria
    {
        public string QuoteId
        {
            get;
            set;
        }
        public string RfqId
        {
            get;
            set;
        }
    }
    #endregion
}
