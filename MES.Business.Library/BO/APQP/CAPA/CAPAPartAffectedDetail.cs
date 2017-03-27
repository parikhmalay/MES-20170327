using Account.DTO.Library;
using MES.Business.Repositories.APQP.CAPA;
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
using System.Data.Entity;

namespace MES.Business.Library.BO.APQP.CAPA
{
    class CAPAPartAffectedDetail : ContextBusinessBase, ICAPAPartAffectedDetailRepository
    {
        public CAPAPartAffectedDetail()
            : base("CAPAPartAffectedDetail")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.APQP.CAPA.capaPartAffectedDetail cAPAPartAffectedDetail)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.capaPartAffectedDetail();
            if (cAPAPartAffectedDetail.Id > 0)
            {
                recordToBeUpdated = this.DataContext.capaPartAffectedDetails.Where(a => a.Id == cAPAPartAffectedDetail.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("CAPANotExists");
                else
                {
                    recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                    recordToBeUpdated.UpdatedBy = CurrentUser;
                    this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                }
            }
            else
            {
                recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                this.DataContext.capaPartAffectedDetails.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.CorrectiveActionId = cAPAPartAffectedDetail.CorrectiveActionId;
                recordToBeUpdated.DefectTypeId = cAPAPartAffectedDetail.DefectTypeId;
                recordToBeUpdated.APQPItemId = cAPAPartAffectedDetail.APQPItemId;
                recordToBeUpdated.PartName = cAPAPartAffectedDetail.PartName;
                recordToBeUpdated.ActualCompletedDate = cAPAPartAffectedDetail.ActualCompletedDate;
                recordToBeUpdated.CustomerRejectedPartQty = cAPAPartAffectedDetail.CustomerRejectedPartQty;
                recordToBeUpdated.SupplierRejectedPartQty = cAPAPartAffectedDetail.SupplierRejectedPartQty;
                recordToBeUpdated.PartsDeliveryLateQty = cAPAPartAffectedDetail.PartsDeliveryLateQty;
                this.DataContext.SaveChanges();
                cAPAPartAffectedDetail.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("CAPASavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, cAPAPartAffectedDetail.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.APQP.CAPA.capaPartAffectedDetail> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int cAPAPartAffectedDetailId)
        {
            //set the out put param
            ObjectParameter ErrorKey = new ObjectParameter("ErrorKey", "");
            this.RunOnDB(context =>
            {
                context.capaDeleteCAPAPartAffectedDetail(cAPAPartAffectedDetailId, ErrorKey);
            });
            if (string.IsNullOrEmpty(Convert.ToString(ErrorKey.Value)))
                return SuccessBoolResponse(Languages.GetResourceText("CAPADeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("CAPADeleteFailed"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.APQP.CAPA.capaPartAffectedDetail>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
    }
}
