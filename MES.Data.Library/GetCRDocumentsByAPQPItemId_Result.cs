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
    
    public partial class GetCRDocumentsByAPQPItemId_Result
    {
        public int Id { get; set; }
        public int ChangeRequestId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
        public string Comments { get; set; }
        public Nullable<int> AuditLogId { get; set; }
    }
}