using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.DefectTracking
{
    public class DefectTrackingDetail : CreateEditPropBase
    {
        public int Id { get; set; }
        public int DefectTrackingId { get; set; }
        public Nullable<int> APQPItemId { get; set; }
        public string PartNumber { get; set; }
        public Nullable<System.DateTime> DateRejected { get; set; }
        public string PartName { get; set; }
        public Nullable<int> CustomerInitialRejectQty { get; set; }
        public string DefectDescription { get; set; }
        public Nullable<System.DateTime> SortedStartDate { get; set; }
        public Nullable<int> CustomerBalanceSortedQty { get; set; }
        public Nullable<int> CustomerAdditionalRejectQty { get; set; }
        public Nullable<int> CustomerTotalReworkedQty { get; set; }
        public Nullable<int> CustomerRejectedPartQty { get; set; }
        public string CorrectiveActionNumber { get; set; }
        public string CorrectiveActionInitiatedBy { get; set; }
        public string CorrectiveActionInitiatedByName { get; set; }
        public Nullable<System.DateTime> CorrectiveActionInitiatedDate { get; set; }
        public Nullable<System.DateTime> CorrectiveActionDueDate { get; set; }
        public Nullable<System.DateTime> ActualCompletedDate { get; set; }
        public string MESWarehouseLocation { get; set; }
        public string MESWarehouseLocationLabel { get; set; }
        public Nullable<int> MESTotalSortedQty { get; set; }
        public Nullable<int> MESRejectDuringSort { get; set; }
        public Nullable<int> MESReworkedQty { get; set; }
        public Nullable<System.DateTime> SortedEndDate { get; set; }
        public Nullable<int> TotalNumberOfPartsRejected { get; set; }
        public Nullable<System.DateTime> CustomerIssuedCreditDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierContactName { get; set; }
        public Nullable<decimal> SortingCost { get; set; }
        public Nullable<System.DateTime> SupplierIssuedDebitDate { get; set; }
        public string Comment { get; set; }
        public string Region { get; set; }
        public string DispositionOfParts { get; set; }
        public string SupplierCode { get; set; }
        public Nullable<int> FinalQtyScrapped { get; set; }
        public Nullable<int> FinalQtyGood { get; set; }
        public string WeightPerPiece { get; set; }
        public Nullable<decimal> SellingPricePerPiece { get; set; }
        public Nullable<short> MESWarehouseLocationId { get; set; }
        public bool chkSelect { get; set; }
    }
}
