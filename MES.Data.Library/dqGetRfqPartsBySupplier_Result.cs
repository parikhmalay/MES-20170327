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
    
    public partial class dqGetRfqPartsBySupplier_Result
    {
        public int Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierState { get; set; }
        public string Country { get; set; }
        public string SentAnyRFQ { get; set; }
        public Nullable<int> NoOfRFQSent { get; set; }
        public Nullable<int> NoOfRFQQuoted { get; set; }
        public Nullable<int> NoOfParts { get; set; }
        public decimal ToolingPrice { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ConversionCost { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public decimal OtherProcessCost { get; set; }
        public Nullable<decimal> TotalDolarQuoted { get; set; }
        public int QuoteWon { get; set; }
    }
}
