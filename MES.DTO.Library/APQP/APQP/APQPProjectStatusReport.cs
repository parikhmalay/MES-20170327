using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class APQPProjectStatusReport
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> ProjectKickoffDate { get; set; }
        public string PartNumber { get; set; }
        public string RevLevel { get; set; }
        public string Description { get; set; }
        public string Customer { get; set; }
        public string ProjectName { get; set; }
        public string Manufacturer { get; set; }
        public string APQPEngineer { get; set; }
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
    }
    public class ReportSearchCriteria
    {
        public string PartNo { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string CustomerName { get; set; }
        public string ProjectName { get; set; }
        public string APQPStatusIds { get; set; }
        public string SAMUserId { get; set; }
        public string APQPQualityEngineerId { get; set; }
        public string SupplyChainCoordinatorId { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerCode { get; set; }
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public bool AllowConfidentialDocumentType { get; set; }
        public string SearchHeading { get; set; }
    }
}
