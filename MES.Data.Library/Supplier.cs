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
    
    public partial class Supplier
    {
        public Supplier()
        {
            this.CommodityTypeSuppliers = new HashSet<CommodityTypeSupplier>();
            this.Documents = new HashSet<Document>();
            this.ShipmentSuppliers = new HashSet<ShipmentSupplier>();
            this.SupplierAssessments = new HashSet<SupplierAssessment>();
            this.Contacts1 = new HashSet<Contacts1>();
            this.CommoditySuppliers = new HashSet<CommoditySupplier>();
            this.AspNetUsers = new HashSet<AspNetUser>();
        }
    
        public int Id { get; set; }
        public string SupplierCode { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<short> CountryId { get; set; }
        public string Zip { get; set; }
        public string Website { get; set; }
        public string CompanyPhone1 { get; set; }
        public string CompanyPhone2 { get; set; }
        public string CompanyFax { get; set; }
        public string Comments { get; set; }
        public short WorkQualityRating { get; set; }
        public short TimelineRating { get; set; }
        public short CostingRating { get; set; }
        public Nullable<short> Status { get; set; }
        public Nullable<bool> IsCurrentSupplier { get; set; }
        public Nullable<System.DateTime> AssessmentDate { get; set; }
        public string Score { get; set; }
        public string SQCertificationFilePath { get; set; }
        public string AssessmentScore { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.Guid> SQId { get; set; }
        public Nullable<int> OldSQId { get; set; }
    
        public virtual ICollection<CommodityTypeSupplier> CommodityTypeSuppliers { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
        public virtual ICollection<ShipmentSupplier> ShipmentSuppliers { get; set; }
        public virtual ICollection<SupplierAssessment> SupplierAssessments { get; set; }
        public virtual ICollection<Contacts1> Contacts1 { get; set; }
        public virtual ICollection<CommoditySupplier> CommoditySuppliers { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
