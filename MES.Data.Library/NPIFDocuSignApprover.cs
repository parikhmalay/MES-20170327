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
    
    public partial class NPIFDocuSignApprover
    {
        public int Id { get; set; }
        public Nullable<int> NPIFDocusignId { get; set; }
        public Nullable<short> DesignationId { get; set; }
        public string UserId { get; set; }
        public string RecipientId { get; set; }
    
        public virtual NPIFDocusign NPIFDocusign { get; set; }
        public virtual Designation Designation { get; set; }
    }
}