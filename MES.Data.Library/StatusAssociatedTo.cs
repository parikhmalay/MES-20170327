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
    
    public partial class StatusAssociatedTo
    {
        public int Id { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> AssociatedToId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        public virtual AssociatedTo AssociatedTo { get; set; }
        public virtual Status Status { get; set; }
    }
}
