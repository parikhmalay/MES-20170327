using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.DefectTracking
{
    public class DefectTracking : CreateEditPropBase
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
        public string PartNumber { get; set; }
        public string Supplier { get; set; }
        public string CustomerRejectedPartQty { get; set; }
        public string RMAFormFilePath { get; set; }
        //public string APQPItemIds { get; set; } //partnumber
        public List<DefectTrackingDetail> lstDefectTrackingDetail { get; set; }
        public string Mode { get; set; }
        public int AddToCAPA { get; set; }
    }
    public class SearchCriteria
    {
        public string RMANumber { get; set; }
        public string CustomerName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        //public Nullable<int> APQPItemId { get; set; } //partNumber
        public Nullable<int> PartNumberId { get; set; } //partNumber
        public string RMAInitiatedBy { get; set; }
        public string CorrectiveActionNumber { get; set; } //CapaNumber
        public Nullable<short> MESWarehouseLocationId { get; set; }
    }
}
