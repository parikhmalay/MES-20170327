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
    
    public partial class GetProjectStatus_Result
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> ProjectKickoffDate { get; set; }
        public string PartNumber { get; set; }
        public string RevLevel { get; set; }
        public string Description { get; set; }
        public string Customer { get; set; }
        public string ProjectName { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<System.DateTime> ActualToolingKickoffDate { get; set; }
        public string ToolingLeadtime { get; set; }
        public Nullable<System.DateTime> PlannedToolingCompletionDate { get; set; }
        public Nullable<System.DateTime> ActualToolingCompletionDate { get; set; }
        public Nullable<System.DateTime> EstimatedSampleShipmentDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string APQPStatus { get; set; }
        public string Category { get; set; }
        public string Stage { get; set; }
        public string QualityFeedback { get; set; }
        public string APQPEngineer { get; set; }
    }
}