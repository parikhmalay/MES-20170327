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
    
    public partial class POPart
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public string PONumber { get; set; }
        public string PartNumber { get; set; }
        public int PartQuantity { get; set; }
        public string PartDescription { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual Shipment Shipment { get; set; }
    }
}
