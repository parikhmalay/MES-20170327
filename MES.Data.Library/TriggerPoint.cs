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
    
    public partial class TriggerPoint
    {
        public TriggerPoint()
        {
            this.TriggerPointUsers = new HashSet<TriggerPointUser>();
            this.TriggerPointUsers1 = new HashSet<TriggerPointUser>();
        }
    
        public int Id { get; set; }
        public string TriggerPoint1 { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual ICollection<TriggerPointUser> TriggerPointUsers { get; set; }
        public virtual ICollection<TriggerPointUser> TriggerPointUsers1 { get; set; }
    }
}
