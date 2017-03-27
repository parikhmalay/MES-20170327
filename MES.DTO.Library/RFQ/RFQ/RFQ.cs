using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQ : CreateEditPropBase
    {
        public string Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int CustomerContactId { get; set; }
        public string CustomerContactName { get; set; }
        public string CustomerContactEmail { get; set; }
        public DateTime Date { get; set; }
        public string rfqDateString { get; set; }
        public decimal ExchangeRate { get; set; }
        public string Currency { get; set; }

        public string OtherAssumption { get; set; }
        public string ProjectName { get; set; }
        public string RawMaterialPriceAssumed { get; set; }
        public Nullable<System.DateTime> QuoteDueDate { get; set; }
        public string Status { get; set; }

        public bool isRevision { get; set; }

        public string SAMId { get; set; }
        public int? ProcessId { get; set; }
        public int? CommodityId { get; set; }
        public string Commodity { get; set; }
        public short? CommodityTypeId { get; set; }
        public string RFQCoordinatorId { get; set; }
        public int? RFQSourceId { get; set; }
        public string RFQCoordinator { get; set; }
        public int? RFQTypeId { get; set; }
        public string RFQType { get; set; }
        public string SupplierRequirement { get; set; }
        public short? RFQPriorityId { get; set; }
        public short? IndustryTypeId { get; set; }
        public int? NonAwardFeedbackId { get; set; }
        public string NonAwardDetailedFeedback { get; set; }
        public DateTime? StatusUpdatedDate { get; set; }
        public string Remarks { get; set; }
        public string RfqFilePath { get; set; }
        public string RfqFileName { get; set; }
        public int QuotesRcvd { get; set; }
        public string daysOld { get; set; }
        public string StatusLegend { get; set; }
        public int? QuotedSupplier { get; set; }
        public List<RFQParts> lstRFQPart { get; set; }

        public List<RFQSuppliers> lstQuotedSuppliers { get; set; }
        public List<MES.DTO.Library.RFQ.Quote.Quote> lstQuoteToCustomer { get; set; }

        public string SupplierName { get; set; }

        public string RfqPDFPath { get; set; }
        public string UploadPartFilePath { get; set; }
    }
    #region Criteria
    public class SearchCriteria
    {
        public string RfqId
        {
            get;
            set;
        }
        public string CompanyName
        {
            get;
            set;
        }
        public string ContactFullName
        {
            get;
            set;
        }
        public string ProjectName
        {
            get;
            set;
        }
        public string PartNumber
        {
            get;
            set;
        }
        public string WorkType
        {
            get;
            set;
        }
        public string rfqCoordinator
        {
            get;
            set;
        }
        public string rfqSource
        {
            get;
            set;
        }
        public string Process
        {
            get;
            set;
        }
        public string Commodity
        {
            get;
            set;
        }
        public string SAM
        {
            get;
            set;
        }
        public string rfqPriority
        {
            get;
            set;
        }
        public string PartDescription
        {
            get;
            set;
        }
        public DateTime? RfqDateFrom
        {
            get;
            set;
        }
        public DateTime? RfqDateTo
        {
            get;
            set;
        }
        //public bool isFirstTimeLoad
        //{
        //    get;
        //    set;
        //}
        public List<ItemList> SAMItems { get; set; }
    }

    #endregion
}
