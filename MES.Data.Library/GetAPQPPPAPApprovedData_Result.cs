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
    
    public partial class GetAPQPPPAPApprovedData_Result
    {
        public Nullable<int> Id { get; set; }
        public string PartNumber { get; set; }
        public Nullable<System.DateTime> ProjectKickoffDate { get; set; }
        public Nullable<System.DateTime> ToolingKickoffDate { get; set; }
        public string ToolingLeadtimeDays { get; set; }
        public string APQPEngineer { get; set; }
        public string SupplyChainCoordinator { get; set; }
        public string SalesAccountManager { get; set; }
        public string CustomerName { get; set; }
        public Nullable<System.DateTime> PlanToolingCompletionDate { get; set; }
        public Nullable<System.DateTime> ActualToolingCompletionDate { get; set; }
        public Nullable<System.DateTime> PSWDate { get; set; }
        public Nullable<System.DateTime> ActualPSWDate { get; set; }
        public Nullable<System.DateTime> PPAPPartsApprovedDate { get; set; }
        public string PPAPStatus { get; set; }
        public string EAUUsage { get; set; }
    }
}