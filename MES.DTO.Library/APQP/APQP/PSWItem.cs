using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class PSWItem
    {
        public int ItemMasterId { get; set; }
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
        public string Status { get; set; }
        public Nullable<bool> IsSourceFromSAP { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<short> DestinationId { get; set; }
        public string Destination { get; set; }
        public string Location { get; set; }
        public string DrawingNumber { get; set; }
        public Nullable<int> ToolingLaunchId { get; set; }
        public string MESToolingPONumber { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> RevisionDate { get; set; }
        public Nullable<System.DateTime> ToolingKickoffDate { get; set; }
        public Nullable<System.DateTime> PlanToolingCompletionDate { get; set; }
        public string APQPDrawingStatus { get; set; }
        public Nullable<int> PPAPSubmissonPreparationDays { get; set; }
        public Nullable<int> ProjectTrackingId { get; set; }
        public Nullable<int> APQPProjectStageId { get; set; }
        public Nullable<System.DateTime> CurrentEstimatedToolingCompletionDate { get; set; }
        public string ShipmentTrackingNumber { get; set; }
        public string QualityFeedbackInformation { get; set; }
        public Nullable<System.DateTime> EstimatedSampleShipmentDate { get; set; }
        public Nullable<int> APQPProjectCategoryId { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> ActualToolingCompletionDate { get; set; }
        public Nullable<int> PPAPSubmissionId { get; set; }
        public Nullable<System.DateTime> PSWDate { get; set; }
        public Nullable<System.DateTime> ActualPSWDate { get; set; }
        public string PPAPStatus { get; set; }
        public string MESWarehouse { get; set; }
        public Nullable<System.DateTime> PartInfoEnteredIntoSAPDate { get; set; }
        public Nullable<System.DateTime> PartInfoEnteredIntoPPEPDate { get; set; }
        public Nullable<System.DateTime> PPAPPartsApprovedDate { get; set; }
        public string PackagingDataFilePath { get; set; }
        public string PSWFilePath { get; set; }
        public Nullable<System.DateTime> PSWFileCreatedDate { get; set; }
        public string Comments { get; set; }
        public string MESAccountManager { get; set; }

        public Nullable<System.DateTime> Intial2DDrawingReceivedDate { get; set; }
        public Nullable<System.DateTime> Intial3DDataReceivedDate { get; set; }
        public string DerivedSupplierAddress { get; set; }
        public string DerivedSupplierName { get; set; }
        public string AdditionalEngineeringChanges { get; set; }
        public Nullable<System.DateTime> AdditionalEngineeringChangesDate { get; set; }

        public string OrgPartNumber { get; set; }
        public string SafetyGovernmentRegulation { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string CheckingAidNo { get; set; }
        public string CheckingAidEngineeringChange { get; set; }
        public Nullable<System.DateTime> CheckingAidEngChangeLevelDate { get; set; }
        public string BuyerCode { get; set; }
        public string Application { get; set; }
        public string MaterialsReporting { get; set; }
        public string SubmittedbyIMDS { get; set; }
        public bool InitialSubmission { get; set; }
        public bool EngineeringChanges { get; set; }
        public bool ToolingTransferReplacementRefurbishmentoradditional { get; set; }
        public bool CorrectionofDiscrepancy { get; set; }
        public bool ToolingInactiveYear { get; set; }
        public bool ChangetoOptionalConstructionorMaterial { get; set; }
        public bool SubSupplierorMaterialSourceChange { get; set; }
        public bool ChangeinPartProcessing { get; set; }
        public bool PartProductedatAdditionalLocation { get; set; }
        public bool OtherPleasespecifybelow { get; set; }
        public string ReasonForSubmissionOther { get; set; }
        public string RequestedForSubmission { get; set; }
        public bool DimensionalMeasurements { get; set; }
        public bool MaterialandFunctionalTests { get; set; }
        public bool AppearanceCriteria { get; set; }
        public bool StatisticalProcessPackage { get; set; }
        public string SubmissionResultRequirement { get; set; }
        public string Rate1 { get; set; }
        public string Rate2 { get; set; }
        public string ExplainationComments { get; set; }
        public string CustomerToolTagged { get; set; }
        public string PrintName { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public Nullable<bool> MOQConfirmation { get; set; }

        public string Commodity { get; set; }
        public string Process  { get; set; }
        public string AdditionalPartDescription { get; set; }
        public string DocumentIds { get; set; }
    }
    public class SAPDataList
    {
        public string ItemCode { get; set; }
        public string RevLevel { get; set; }
        public string PartDec { get; set; }
        public string RevDate { get; set; }
        public string Customer { get; set; }
        public string Supplier { get; set; }
        public string MaterialType { get; set; }
        public string PartWeight { get; set; }
        public decimal PurchasePieceCost { get; set; }
        public decimal SellingPiecePrice { get; set; }
        public string SupplyChainCoordinator { get; set; }
        public string MESToolingPONumber { get; set; }
        public string ToolingKickoffDate { get; set; }
        public string MESWarehouse { get; set; }
        public string APQPEngineer { get; set; }
        public string MESAccountManager { get; set; }

    }
}
