using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQSupplierPartQuote : CreateEditPropBase
    {

        #region "Extra properties"
        public string CustomerPartNo { get; set; }
        public string PartDescription { get; set; }
        public string AdditionalPartDesc { get; set; }
        public int? EstimatedQty { get; set; }
        public List<DTO.Library.RFQ.RFQ.RFQPartAttachment> Specifications { get; set; }
        public string MaterialType { get; set; }
        public decimal? PartWeightKG { get; set; }
        public string RfqId { get; set; }
        public Nullable<DateTime> RFQDate { get; set; }
        public Nullable<DateTime> QuoteDueDate { get; set; }
        public string RFQRemarks { get; set; }
        public int? SupplierId { get; set; }
        public string CompanyName { get; set; }
        public bool IsMandatory { get; set; }
        public decimal? Minimum { get; set; }
        public string UniqueUrl { get; set; }
        public bool IsQuoteTypeDQ { get; set; }
        public bool NoQuote { get; set; }
        public bool IsHistory { get; set; }

        #endregion

        public int Id { get; set; }
        public int RFQSupplierId { get; set; }
        public int RFQPartId { get; set; }
        public Nullable<int> ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerName { get; set; }
        public Nullable<decimal> MaterialCost { get; set; }
        public Nullable<decimal> ProcessCost { get; set; }
        public decimal UnitPrice { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public string Currency { get; set; }
        public string RawMaterialPriceAssumed { get; set; }
        public string QuoteAttachmentFilePath { get; set; }
        public Nullable<decimal> MachiningCost { get; set; }
        public Nullable<decimal> OtherProcessCost { get; set; }
        public string Remarks { get; set; }
        public string SupplierRequirement { get; set; }
        public string SupplierToolingLeadtime { get; set; }
        public string ToolingWarranty { get; set; }
        public Nullable<int> NoOfCavities { get; set; }
        public Nullable<int> MinOrderQty { get; set; }
        public Nullable<bool> MOQConfirmation { get; set; }
        public Nullable<decimal> ExchangeRate { get; set; }
        public string UpdatedDateString { get; set; }
        public string UploadQuoteFilePath { get; set; }
    }
    public class RFQSupplierParQuoteSearchCriteria
    {
        public string RFQId { get; set; }
        public int SupplierId { get; set; }
        public int CustomerId { get; set; }
        public bool IsQuoteTypeDQ { get; set; }
        public string UniqueUrl { get; set; }
        public string UploadQuoteFilePath { get; set; }
    }
    public class RFQSupplierPartQuoteDQ : RFQSupplierPartQuote
    {
        #region "SupplierPartQuote properties inherited over here"
        #endregion

        #region "Rest 7 tables properties (Extra Properties)"
        //dqRawMaterial
        public RFQdqRawMaterial rFQdqRawMaterial { get; set; }
        //dqPrimaryProcessConversion
        public RFQdqPrimaryProcessConversion rFQdqPrimaryProcessConversion { get; set; }
        //dqMachining
        public RFQdqMachining rFQdqMachining { get; set; }
        //dqMachiningSecondaryOperation
        public RFQdqMachiningSecondaryOperation rFQdqMachiningSecondaryOperation { get; set; }
        //dqMachiningOtherOperation
        public RFQdqMachiningOtherOperation rFQdqMachiningOtherOperation { get; set; }
        //dqSurfaceTreatment
        public RFQdqSurfaceTreatment rFQdqSurfaceTreatment { get; set; }
        //dqRawMaterial
        public RFQdqOverhead rFQdqOverhead { get; set; }
        #endregion

    }
}
