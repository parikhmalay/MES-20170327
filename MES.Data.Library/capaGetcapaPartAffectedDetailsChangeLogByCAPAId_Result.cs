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
    
    public partial class capaGetcapaPartAffectedDetailsChangeLogByCAPAId_Result
    {
        public int AuditLogId { get; set; }
        public Nullable<int> ReferenceId { get; set; }
        public string PartNumber { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
        public string UserBy { get; set; }
        public int DetailId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string FieldAlias { get; set; }
        public Nullable<bool> IsPricingField { get; set; }
        public string OperationType { get; set; }
        public string SourceFrom { get; set; }
    }
}
