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
    
    public partial class Country
    {
        public Country()
        {
            this.Divisions = new HashSet<Division>();
            this.Suppliers = new HashSet<Supplier>();
            this.Addresses = new HashSet<Address>();
            this.AspNetUsers = new HashSet<AspNetUser>();
        }
    
        public short Id { get; set; }
        public string Value { get; set; }
    
        public virtual ICollection<Division> Divisions { get; set; }
        public virtual ICollection<Supplier> Suppliers { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}