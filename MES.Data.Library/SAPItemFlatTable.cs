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
    using System.Collections.Generic;
    
    public partial class SAPItemFlatTable
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ProjectName { get; set; }
        public string MaterialType { get; set; }
        public string ItemWeight { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCity { get; set; }
        public string SupplierState { get; set; }
        public string SupplierCountry { get; set; }
        public string Cost { get; set; }
        public string SalesPrice { get; set; }
        public Nullable<bool> IsAPQPItem { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
