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
    
    public partial class PPAPSubmission
    {
        public int Id { get; set; }
        public Nullable<int> APQPItemId { get; set; }
        public Nullable<System.DateTime> PSWDate { get; set; }
        public Nullable<System.DateTime> ActualPSWDate { get; set; }
        public string PPAPStatus { get; set; }
        public string MESWarehouse { get; set; }
        public Nullable<System.DateTime> PartInfoEnteredIntoSAPDate { get; set; }
        public Nullable<System.DateTime> PartInfoEnteredIntoPPEPDate { get; set; }
        public Nullable<System.DateTime> PPAPPartsApprovedDate { get; set; }
        public string PackagingDataFilePath { get; set; }
        public string PSWFilePath { get; set; }
        public Nullable<System.DateTime> PSWFileCreatedDate { get; set; }
        public string Comments { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
