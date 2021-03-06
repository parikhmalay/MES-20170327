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
    
    public partial class Document
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string DocumentFilePath { get; set; }
        public string Comment { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string DocumentFileName { get; set; }
    
        public virtual DocumentType DocumentType { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
