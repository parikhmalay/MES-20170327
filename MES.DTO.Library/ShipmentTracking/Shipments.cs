using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.ShipmentTracking
{
    public class Shipments : CreateEditPropBase
    {
        public int Id { get; set; }
        public short DestinationId { get; set; }
        public string Destination { get; set; }
        public short ForwarderId { get; set; }
        public string ForwarderName { get; set; }
        public Nullable<System.DateTime> EstShpmntDate { get; set; }
        public Nullable<System.DateTime> ActShpmntDate { get; set; }
        public Nullable<System.DateTime> ETAAtWarehouseAtDest { get; set; }
        public Nullable<System.DateTime> ActArrDateAtWarehouseAtDest { get; set; }
        public Nullable<System.DateTime> EstForwarderPickupDate { get; set; }
        public Nullable<System.DateTime> ActForwarderPickupDate { get; set; }
        public bool IsLateDeliveryToForwarder { get; set; }
        public string LateDeliveryToForwarder { get; set; }
        public string InspectionFileUploaded { get; set; }
        public string Documents { get; set; }
        public string PartNumber { get; set; }

        public string Inspector { get; set; }
        public Nullable<System.DateTime> InspectionDate { get; set; }
        public string InspectionReportFilePath { get; set; }
        public string InspectionReportFileName { get; set; }

        public short ShipmentModeId { get; set; }
        public string ShipmentMode { get; set; }
        public Nullable<short> QualityReviewStatusId { get; set; }
        public string QualityReviewStatus { get; set; }
        public string QualityReviewComments { get; set; }
        public Nullable<short> OriginId { get; set; }
        public string Origin { get; set; }
        public bool Status { get; set; }
        public string ContainerNumber { get; set; }
        public string InvoiceNumbers { get; set; }
        public string SupplierNames { get; set; }
        public string PONumbers { get; set; }
        public bool IsShipmentReviewed { get; set; }
        public string ShipmentReviewed { get; set; }
        public bool IsDeleted { get; set; }

        public virtual List<MES.DTO.Library.Common.LookupFields> ShipmentSuppliers { get; set; }
        public virtual List<POParts> lstPOPart { get; set; }
        public virtual List<Documents> lstDocument { get; set; }
        public bool chkSelect { get; set; }

        public string uploadShipmentFilePath { get; set; }
    }

    public class SearchCriteria
    {
        public int? Id { get; set; }
        public bool? IsLateDeliveryToForwarder { get; set; }
        public short? ShipmentMode { get; set; }
        public int? SupplierId { get; set; }
        public short? OriginId { get; set; }
        public short? ForwarderId { get; set; }
        public short? QualityReviewStatusId { get; set; }
        public bool? Status { get; set; }
        public string PONumber { get; set; }
        public string PartNumber { get; set; }
        public string Inspector { get; set; }
        public Nullable<System.DateTime> EstShpmntDateFrom { get; set; }
        public Nullable<System.DateTime> EstShpmntDateTo { get; set; }
        public Nullable<System.DateTime> ETAAtWarehouseAtDestFrom { get; set; }
        public Nullable<System.DateTime> ETAAtWarehouseAtDestTo { get; set; }
        public Nullable<System.DateTime> ActArrDateAtWarehouseAtDestFrom { get; set; }
        public Nullable<System.DateTime> ActArrDateAtWarehouseAtDestTo { get; set; }
        public Nullable<System.DateTime> InspectionDateFrom { get; set; }
        public Nullable<System.DateTime> InspectionDateTo { get; set; }
    }
}
