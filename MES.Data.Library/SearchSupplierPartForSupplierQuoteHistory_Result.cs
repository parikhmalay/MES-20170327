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
    
    public partial class SearchSupplierPartForSupplierQuoteHistory_Result
    {
        public int Id { get; set; }
        public string CustomerPartNo { get; set; }
        public string PartDescription { get; set; }
        public string AdditionalPartDescription { get; set; }
        public Nullable<int> EstimatedQty { get; set; }
        public string MaterialType { get; set; }
        public Nullable<decimal> PartWeightKG { get; set; }
        public string RFQId { get; set; }
        public Nullable<System.DateTime> RFQDate { get; set; }
        public Nullable<System.DateTime> QuoteDueDate { get; set; }
        public string RFQRemarks { get; set; }
        public Nullable<int> RFQSupplierId { get; set; }
        public Nullable<bool> IsQuoteTypeDQ { get; set; }
        public bool NoQuote { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public string CompanyName { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ProcessCost { get; set; }
        public decimal UnitPrice { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public Nullable<decimal> OtherProcessCost { get; set; }
        public int SupplierPartQuoteId { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public string RawMaterialPriceAssumed { get; set; }
        public string QuoteAttachmentFilePath { get; set; }
        public string ToolingWarranty { get; set; }
        public string SupplierToolingLeadtime { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public Nullable<int> NoOfCavities { get; set; }
        public Nullable<int> MinOrderQty { get; set; }
        public Nullable<bool> MOQConfirmation { get; set; }
        public Nullable<int> RawMaterialId { get; set; }
        public string RawMaterialDesc { get; set; }
        public Nullable<decimal> RawMatInputInKg { get; set; }
        public Nullable<decimal> RawMatCostPerKg { get; set; }
        public Nullable<decimal> MfgRejectRate { get; set; }
        public Nullable<decimal> RawMatTotal { get; set; }
        public Nullable<int> PrimaryProcessId { get; set; }
        public Nullable<int> PPCMachineDescId { get; set; }
        public string PPCMachineDescription { get; set; }
        public Nullable<int> PPCMachineSize { get; set; }
        public Nullable<int> PPCCycleTime { get; set; }
        public Nullable<decimal> PPCManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> ProcessConversionCostPerPart { get; set; }
        public Nullable<int> MachiningId { get; set; }
        public Nullable<int> MMachiningDescId { get; set; }
        public string MMachiningDescription { get; set; }
        public Nullable<int> MCycleTime { get; set; }
        public Nullable<decimal> MManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> MachiningCostPerPart { get; set; }
        public Nullable<int> MachiningSecOperationId { get; set; }
        public Nullable<int> MSOSecondaryOperationDescId { get; set; }
        public string MSOSecondaryOperationDescription { get; set; }
        public Nullable<int> MSOCycleTime { get; set; }
        public Nullable<decimal> MSOManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> MSOSecondaryCostPerPart { get; set; }
        public Nullable<int> MachiningOtherOperationId { get; set; }
        public Nullable<int> MOOSecondaryOperationDescId { get; set; }
        public string MOOSecondaryOperationDescription { get; set; }
        public Nullable<int> MOOCycleTime { get; set; }
        public Nullable<decimal> MOOManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> MOOSecondaryCostPerPart { get; set; }
        public Nullable<int> SurfaceTreatmentId { get; set; }
        public Nullable<int> STCoatingTypeId { get; set; }
        public string STCoatingType { get; set; }
        public Nullable<int> STPartsPerHour { get; set; }
        public Nullable<decimal> STManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> STCoatingCostPerHour { get; set; }
        public Nullable<int> OverheadId { get; set; }
        public Nullable<decimal> OverheadInventoryCarryingCost { get; set; }
        public Nullable<decimal> OverheadPacking { get; set; }
        public Nullable<decimal> OverheadPackagingMaterial { get; set; }
        public Nullable<decimal> OverheadLocalFreightToPort { get; set; }
        public Nullable<decimal> OverheadProfitAndSGA { get; set; }
        public Nullable<decimal> OverheadPercentPiecePrice { get; set; }
        public string UpdatedDateString { get; set; }
        public string RawMaterialIndexUsed { get; set; }
        public Nullable<decimal> MaterialLoss { get; set; }
    }
}