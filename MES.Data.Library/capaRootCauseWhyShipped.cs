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
    
    public partial class capaRootCauseWhyShipped
    {
        public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public short QueryId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Reason { get; set; }
    
        public virtual capaItemMaster capaItemMaster { get; set; }
        public virtual capaQuery capaQuery { get; set; }
    }
}
