using Account.DTO.Library;
using MES.Business.Repositories.RFQ.RFQ;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;

namespace MES.Business.Library.BO.RFQ.RFQ
{
    class RFQdqMachiningOtherOperation : ContextBusinessBase, IRFQdqMachiningOtherOperationRepository
    {
        public RFQdqMachiningOtherOperation()
            : base("RFQdqMachiningOtherOperation")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation rFQdqMachiningOtherOperation)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.dqMachiningOtherOperation();

            if (rFQdqMachiningOtherOperation.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dqMachiningOtherOperations.Where(a => a.Id == rFQdqMachiningOtherOperation.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQdqMachiningOtherOperationNotExists");
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
                this.DataContext.dqMachiningOtherOperations.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.RFQSupplierPartDQId = rFQdqMachiningOtherOperation.RFQSupplierPartDQId;
                recordToBeUpdated.SecondaryOperationDescId = rFQdqMachiningOtherOperation.SecondaryOperationDescId;
                if (rFQdqMachiningOtherOperation.SecondaryOperationDescId.HasValue)
                {
                    recordToBeUpdated.SecondaryOperationDescription = this.DataContext.SecondaryOperationDescs.Where(cm => cm.Id == rFQdqMachiningOtherOperation.SecondaryOperationDescId.Value && cm.IsDeleted == false).FirstOrDefault().SecondaryOperationDescription;
                }
                else
                    recordToBeUpdated.SecondaryOperationDescription = rFQdqMachiningOtherOperation.SecondaryOperationDescription;                
                recordToBeUpdated.CycleTime = rFQdqMachiningOtherOperation.CycleTime;
                recordToBeUpdated.ManPlusMachineRatePerHour = rFQdqMachiningOtherOperation.ManPlusMachineRatePerHour;
                recordToBeUpdated.SecondaryCostPerPart = rFQdqMachiningOtherOperation.SecondaryCostPerPart;
                this.DataContext.SaveChanges();
                rFQdqMachiningOtherOperation.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("RFQdqMachiningOtherOperationSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, rFQdqMachiningOtherOperation.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQdqMachiningOtherOperationId)
        {
            var RFQdqMachiningOtherOperationToBeDeleted = this.DataContext.dqMachiningOtherOperations.Where(a => a.Id == RFQdqMachiningOtherOperationId).SingleOrDefault();
            if (RFQdqMachiningOtherOperationToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQdqMachiningOtherOperationNotExists"));
            }
            else
            {
                RFQdqMachiningOtherOperationToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqMachiningOtherOperationToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqMachiningOtherOperationToBeDeleted).State = EntityState.Modified;
                //RFQdqMachiningOtherOperationToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQdqMachiningOtherOperationDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation>> GetRFQdqMachiningOtherOperationList(int RFQSupplierPartDQId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation> lstRFQdqMachiningOtherOperation = new List<DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation>();
            DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation rFQdqMachiningOtherOperation;
            this.RunOnDB(context =>
             {
                 var rFQdqMachiningOtherOperationList = context.dqMachiningOtherOperations.Where(c => c.RFQSupplierPartDQId == RFQSupplierPartDQId).ToList();
                 if (rFQdqMachiningOtherOperationList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     foreach (var item in rFQdqMachiningOtherOperationList)
                     {
                         rFQdqMachiningOtherOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation();
                         rFQdqMachiningOtherOperation.Id = item.Id;
                         rFQdqMachiningOtherOperation.RFQSupplierPartDQId = item.RFQSupplierPartDQId;
                         lstRFQdqMachiningOtherOperation.Add(rFQdqMachiningOtherOperation);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQdqMachiningOtherOperation>>(errMSg, lstRFQdqMachiningOtherOperation);
            return response;
        }

    }
}
