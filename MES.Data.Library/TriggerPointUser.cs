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
    
    public partial class TriggerPointUser
    {
        public int Id { get; set; }
        public int TriggerPointId { get; set; }
        public string UserId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> OldUserId { get; set; }
    
        public virtual TriggerPoint TriggerPoint { get; set; }
        public virtual TriggerPoint TriggerPoint1 { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}