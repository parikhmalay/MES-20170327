using Account.DTO.Library;
using MES.Business.Repositories.APQP.DefectTracking;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;
using MES.Business.Mapping.Extensions;
using System.Net;

namespace MES.Business.Library.BO.APQP.DefectTracking
{
    class DefectTrackingDetail : ContextBusinessBase, IDefectTrackingDetailRepository
    {
        public DefectTrackingDetail()
            : base("DefectTrackingDetail")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.DefectTracking.DefectTrackingDetail defectTrackingDetail)
        {
            string errMSg = null;
            string successMsg = null;

            ObjectParameter DefectTrackingDetailId = new ObjectParameter("DefectTrackingDetailId", defectTrackingDetail.Id);
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            this.RunOnDB(context =>
            {
                int result = context.dtSaveDefectTrackingDetails(DefectTrackingDetailId, defectTrackingDetail.DefectTrackingId, defectTrackingDetail.APQPItemId, defectTrackingDetail.DateRejected, defectTrackingDetail.PartName,
                    defectTrackingDetail.CustomerInitialRejectQty, defectTrackingDetail.DefectDescription, defectTrackingDetail.SortedStartDate, defectTrackingDetail.CustomerBalanceSortedQty,
                    defectTrackingDetail.CustomerAdditionalRejectQty, defectTrackingDetail.CustomerTotalReworkedQty, defectTrackingDetail.CustomerRejectedPartQty, defectTrackingDetail.CorrectiveActionNumber,
                    defectTrackingDetail.CorrectiveActionInitiatedBy, defectTrackingDetail.CorrectiveActionInitiatedDate, defectTrackingDetail.CorrectiveActionDueDate, defectTrackingDetail.ActualCompletedDate,
                    defectTrackingDetail.MESWarehouseLocation, defectTrackingDetail.MESWarehouseLocationId, defectTrackingDetail.MESTotalSortedQty, defectTrackingDetail.MESRejectDuringSort,
                    defectTrackingDetail.MESReworkedQty, defectTrackingDetail.SortedEndDate, defectTrackingDetail.TotalNumberOfPartsRejected, defectTrackingDetail.CustomerIssuedCreditDate,
                    defectTrackingDetail.SupplierCode, defectTrackingDetail.SupplierName, defectTrackingDetail.SupplierContactName, defectTrackingDetail.SortingCost, defectTrackingDetail.SupplierIssuedDebitDate,
                    defectTrackingDetail.Comment, defectTrackingDetail.Region, defectTrackingDetail.DispositionOfParts, defectTrackingDetail.FinalQtyScrapped, defectTrackingDetail.FinalQtyGood,
                    defectTrackingDetail.WeightPerPiece, defectTrackingDetail.SellingPricePerPiece, CurrentUser, ErrorKey);
                if (Convert.ToInt32(DefectTrackingDetailId.Value) <= 0 || !string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                {
                    errMSg = Languages.GetResourceText("DTSaveFail");
                }
                else
                {
                    defectTrackingDetail.Id = Convert.ToInt32(DefectTrackingDetailId.Value);
                }
            });
            if (string.IsNullOrEmpty(errMSg))
            {
                successMsg = Languages.GetResourceText("DTSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, defectTrackingDetail.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.DefectTracking.DefectTrackingDetail> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int defectTrackingDetailId)
        {
            //set the out put param
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            this.RunOnDB(context =>
            {
                context.dtDeleteDefectTrackingDetail(defectTrackingDetailId, ErrorKey);
            });
            if (string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                return SuccessBoolResponse(Languages.GetResourceText("DTDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("DTDeleteFailed"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.DefectTracking.DefectTrackingDetail>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.DefectTracking.DefectTrackingDetail>> GetDefectTrackingDetailList(int defectTrackingId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.APQP.DefectTracking.DefectTrackingDetail> lstDefectTracking = new List<DTO.Library.APQP.DefectTracking.DefectTrackingDetail>();
            DTO.Library.APQP.DefectTracking.DefectTrackingDetail defectTrackingDetail;
            this.RunOnDB(context =>
             {
                 var DefectTrackingList = context.dtGetDefectTrackingDetails(defectTrackingId).ToList();

                 if (DefectTrackingList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     foreach (var item in DefectTrackingList)
                     {
                         defectTrackingDetail = new DTO.Library.APQP.DefectTracking.DefectTrackingDetail();
                         defectTrackingDetail = ObjectLibExtensions.AutoConvert<DTO.Library.APQP.DefectTracking.DefectTrackingDetail>(item);
                         lstDefectTracking.Add(defectTrackingDetail);
                     }
                 }
             });
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.APQP.DefectTracking.DefectTrackingDetail>>(errMSg, lstDefectTracking);
            return response;
        }
    }
}
