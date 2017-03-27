using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class ToolingLaunch : CreateEditPropBase
    {
        #region Properties
        public Nullable<int> Id { get; set; }
        public Nullable<int> APQPItemId { get; set; }
        public string MESToolingPONumber { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> RevisionDate { get; set; }
        public Nullable<System.DateTime> ToolingKickoffDate { get; set; }
        public Nullable<System.DateTime> PlanToolingCompletionDate { get; set; }
        public string APQPDrawingStatus { get; set; }
        public Nullable<int> PPAPSubmissonPreparationDays { get; set; }
        public string Comments { get; set; }
        public string UpdatedFromSource { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public string ManufacturerName { get; set; }
        public string PartNumber { get; set; }
        public string PartDesc { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string Status { get; set; }
        public string DrawingNumber { get; set; }
        public string DaysOld { get; set; }
        public string PNDaysOld { get; set; }
        public int IsDocument { get; set; }
        #endregion
    }
}
