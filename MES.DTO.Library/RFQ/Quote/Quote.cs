using MES.DTO.Library.Common;
using MES.DTO.Library.Setup.DocumentType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.Quote
{
    public class Quote : CreateEditPropBase
    {
        public string Id { get; set; }

        public string RfqId { get; set; }
        public int CustomerId { get; set; }
        public string ParentID { get; set; }
        public string pQuoteId { get; set; }
        public string QuoteNumber { get; set; }
        public DateTime? Date { get; set; }
        public bool Revision { get; set; }
        public string SAMId { get; set; }
        public string SAMUser { get; set; }
        public string ProjectName { get; set; }

        public decimal? SupplierCost { get; set; }

        public decimal? CustomDutiesPercent { get; set; }
        public decimal? ShippingCostPercent { get; set; }
        public decimal? WarehousingPercent { get; set; }
        public decimal? SGAProfitPercent { get; set; }
        public decimal? SalesCommissionPercent { get; set; }
        public decimal? ToolingCostPercent { get; set; }
        public string ShippingCostCalMethod { get; set; }

        public decimal? AmountWon { get; set; }
        public short? StatusId { get; set; }
        public decimal? Amount { get; set; }

        public string QuoteFilePath { get; set; }
        public string ExtQuoteFilePath { get; set; }

        public string GeneralAssumption { get; set; }
        public string Comments { get; set; }
        public List<CommentsLookUp> QuoteAssumptionList { get; set; }
        public List<CommentsLookUp> MESCommentsList { get; set; }
        public string MESComments { get; set; }

        public bool isRevision { get; set; }
        public string StatusLegend { get; set; }

        public string ContactFullName { get; set; }
        public string CompanyName { get; set; }

        public bool IsExcelTypeExt { get; set; }
        public List<QuoteDetails> lstQuoteDetails { get; set; }
        public List<QuoteCalculationHistory> lstQuoteCalculationHistory { get; set; }
        public Quote quotesGeneralDetails { get; set; }
    }
    #region Criteria
    public class SearchCriteria
    {
        public string QuoteId
        {
            get;
            set;
        }
        public string RfqId
        {
            get;
            set;
        }
        public string CustomerName
        {
            get;
            set;
        }
        public string ContactFullName
        {
            get;
            set;
        }
        public string PartNumber
        {
            get;
            set;
        }
        public string rfqCoordinator
        {
            get;
            set;
        }
        public string SAM
        {
            get;
            set;
        }
        public DateTime? QuoteDateFrom
        {
            get;
            set;
        }
        public DateTime? QuoteDateTo
        {
            get;
            set;
        }
    }
    #endregion

    public class CommentsLookUp
    {
        #region Model property
        public int Id { get; set; }
        public string Name { get; set; }
        #endregion
    }
}
