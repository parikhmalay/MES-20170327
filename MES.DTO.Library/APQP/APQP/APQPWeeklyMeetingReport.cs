using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class PPAPApprovalReport
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
    public class PPAPApprovalReportSearchCriteria
    {
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public string CustomerName { get; set; }
        public string APQPQualityEngineerIds { get; set; }
        public string SupplyChainCoordinatorIds { get; set; }
        public string SAMUserIds { get; set; }
        public string ProjectStageIds { get; set; }
        public string PPAPStatus { get; set; }
        public string PPAPStatusText { get; set; }
        public string SearchHeading { get; set; }
    }
}
