//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MES.Data.Library
{
    using System;
    
    public partial class GetRfqPartCostingComparisons_Result
    {
        public string RFQNo { get; set; }
        public Nullable<int> RSPQId { get; set; }
        public int RfqPartid { get; set; }
        public string CustomerPartNo { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string SupplierName { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ProcessCost { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public Nullable<decimal> OtherProcessCost { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<int> RFQSupplierPartQuoteId { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public string Currency { get; set; }
        public string RawMaterialPriceAssumed { get; set; }
        public string QuoteAttachmentFilePath { get; set; }
        public Nullable<decimal> SupplierCostPerKg { get; set; }
    }
}