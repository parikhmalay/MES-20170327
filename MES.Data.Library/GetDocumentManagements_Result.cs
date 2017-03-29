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
    
    public partial class GetDocumentManagements_Result
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
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> RevisionDate { get; set; }
        public int IsDocument { get; set; }
        public string DrawingNumber { get; set; }
    }
}