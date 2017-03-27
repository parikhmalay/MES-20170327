using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.DefectTracking
{
    public class DefectTrackingReport : CreateEditPropBase
    {
        public int Id { get; set; }
        public string IncludeInPPM { get; set; }
        public string Finding { get; set; }
        public string QualityOrDeliveryIssue { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string RMANumber { get; set; }
        public Nullable<System.DateTime> RMADate { get; set; }
        public string RMAInitiatedBy { get; set; }
        public string RMAInitiatedByName { get; set; }
        public Nullable<int> TotalItems { get; set; }
        public List<DefectTrackingReportDetails> lstDefectTrackingReportDetails { get; set; }
    }
    public class DefectTrackingReportDetails
    {
        public string IncludeInPPM { get; set; }
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string SupplierName { get; set; }
        public int CustomerRejectedPartQty { get; set; }
        public int DefectTrackingId { get; set; }
    }
    public class ReportSearchCriteria
    {
        public string RMANumber { get; set; }
        public string CustomerName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Nullable<int> PartNo { get; set; } //partNumber
        public string RMAInitiatedBy { get; set; }
        public string CAPANumber { get; set; } //CorrectiveActionNumber
        public string MESWarehouseLocation { get; set; }
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
    }
}
