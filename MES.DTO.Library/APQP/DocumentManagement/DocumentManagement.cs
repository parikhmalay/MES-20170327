using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.DocumentManagement
{
    public class DocumentManagement : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<System.DateTime> CustomerToolingPOAuthRcvdDate { get; set; }
        public Nullable<System.DateTime> ProjectKickoffDate { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerState { get; set; }
        public string CustomerManufacturingLocation { get; set; }
        public Nullable<int> CustomerProjectLeadId { get; set; }
        public string CustomerProjectLead { get; set; }
        public string CustomerProjectLeadEmail { get; set; }
        public string CustomerProjectLeadPhone { get; set; }
        public Nullable<int> CustomerEngineerId { get; set; }
        public string CustomerEngineer { get; set; }
        public string CustomerEngineerEmail { get; set; }
        public string CustomerEngineerPhone { get; set; }
        public string ShipToLocation { get; set; }
        public string PPAPSubmissionLevel { get; set; }
        public string NumberOfSampleRequired { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress1 { get; set; }
        public string SupplierAddress2 { get; set; }
        public string SupplierCity { get; set; }
        public string SupplierState { get; set; }
        public string SupplierCountry { get; set; }
        public string SupplierZip { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string ManufacturerCode { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerAddress1 { get; set; }
        public string ManufacturerAddress2 { get; set; }
        public string ManufacturerCity { get; set; }
        public string ManufacturerState { get; set; }
        public string ManufacturerCountry { get; set; }
        public string ManufacturerZip { get; set; }
        public string EAUUsage { get; set; }
        public string PartNumber { get; set; }
        public string PartDesc { get; set; }
        public string MaterialType { get; set; }
        public string PartWeight { get; set; }
        public string ProjectNotes { get; set; }
        public string CustomerToolingPONumber { get; set; }
        public decimal PurchasePieceCost { get; set; }
        public decimal PurchaseToolingCost { get; set; }
        public decimal SellingPiecePrice { get; set; }
        public decimal SellingToolingPrice { get; set; }
        public string ToolingLeadtimeDays { get; set; }
        public string APQPEngineerId { get; set; }
        public string APQPEngineer { get; set; }
        public string SupplyChainCoordinatorId { get; set; }
        public string SupplyChainCoordinator { get; set; }
        public string PartClassification { get; set; }
        public Nullable<short> WorkTypeId { get; set; }
        public string WorkType { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string SAMUser { get; set; }
        public string SAMUserId { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsSourceFromSAP { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> RevisionDate { get; set; }
        public int IsDocument { get; set; }
        public string DrawingNumber { get; set; }
    }
    public class SearchCriteria
    {
        public string PartNo { get; set; }
        public string PartDescription { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public string APQPStatusIds { get; set; }
        public string SAMUserId { get; set; }
        public string APQPQualityEngineerId { get; set; }
        public string SupplyChainCoordinatorId { get; set; }
        public bool AllowConfidentialDocumentType { get; set; }
    }
}
