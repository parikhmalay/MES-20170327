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
    
    public partial class GetDocuments_Result
    {
        public int Id { get; set; }
        public int APQPItemId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public bool IsConfidential { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> PreparedDate { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
        public string Comments { get; set; }
        public string RevLevel { get; set; }
        public Nullable<bool> IsAddPSW { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string DrawingNumber { get; set; }
    }
}
