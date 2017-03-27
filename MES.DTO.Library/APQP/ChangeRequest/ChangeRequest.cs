using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.ChangeRequest
{
    public class ChangeRequest : CreateEditPropBase
    {
        #region Properties
        public int Id { get; set; }
        public int APQPItemId { get; set; }
        public string SourceOfChange { get; set; }
        public string DescriptionOfChange { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> DrawingRevDate { get; set; }
        public Nullable<System.DateTime> MfgStartDateForNewRev { get; set; }
        public string Subject { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string Status { get; set; }
        public string AssignedToId { get; set; }
        public string AssignedToUser { get; set; }
        public Nullable<bool> IsChangeApproved { get; set; }
        public Nullable<bool> IsChangeImplemented { get; set; }
        public decimal apqpPurchasePieceCost { get; set; }
        public decimal apqpSellingPiecePrice { get; set; }
        public decimal apqpPurchaseToolingCost { get; set; }
        public decimal apqpSellingToolingPrice { get; set; }
        public decimal PurchasePieceCost { get; set; }
        public decimal SellingPiecePrice { get; set; }
        public decimal PurchaseToolingCost { get; set; }
        public decimal SellingToolingPrice { get; set; }
        public string WatcherIds { get; set; }
        public string DrawingNumber { get; set; }
        public string crHeader { get; set; }
        public string PartNumber { get; set; }
        public string PartDesc { get; set; }
        public List<DTO.Library.APQP.ChangeRequest.Document> lstDocument { get; set; }
        public virtual List<MES.DTO.Library.Common.LookupFields> WatcherItems { get; set; }

        public bool HasPricingFieldsAccess { get; set; }
        public bool AllowConfidentialDocumentType { get; set; }
        public bool AllowDeleteRecord { get; set; }

        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }
        #endregion
    }
    public class SearchCriteria
    {
        public int? ChangeRequestId { get; set; }
        public int? PartNumberId { get; set; }
        public string StatusIds { get; set; }
        public string AssignedToIds { get; set; }
        public int? Updated { get; set; }
        public string RevLevel { get; set; }

        public string PartNumber { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string CustomerName { get; set; }

        public List<ItemList> AssignedToItems { get; set; }
        //public bool isFirstTimeLoad { get; set; }
    }
}
