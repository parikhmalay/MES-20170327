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
    class RFQdqMachiningSecondaryOperation : ContextBusinessBase, IRFQdqMachiningSecondaryOperationRepository
    {
        public RFQdqMachiningSecondaryOperation()
            : base("RFQdqMachiningSecondaryOperation")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation rFQdqMachiningSecondaryOperation)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.dqMachiningSecondaryOperation();

            if (rFQdqMachiningSecondaryOperation.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dqMachiningSecondaryOperations.Where(a => a.Id == rFQdqMachiningSecondaryOperation.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQdqMachiningSecondaryOperationNotExists");
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
                this.DataContext.dqMachiningSecondaryOperations.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.RFQSupplierPartDQId = rFQdqMachiningSecondaryOperation.RFQSupplierPartDQId;
                recordToBeUpdated.SecondaryOperationDescId = rFQdqMachiningSecondaryOperation.SecondaryOperationDescId;
                if (rFQdqMachiningSecondaryOperation.SecondaryOperationDescId.HasValue)
                {
                    recordToBeUpdated.SecondaryOperationDescription = this.DataContext.SecondaryOperationDescs.Where(cm => cm.Id == rFQdqMachiningSecondaryOperation.SecondaryOperationDescId.Value && cm.IsDeleted == false).FirstOrDefault().SecondaryOperationDescription;
                }
                else
                    recordToBeUpdated.SecondaryOperationDescription = rFQdqMachiningSecondaryOperation.SecondaryOperationDescription;
                recordToBeUpdated.CycleTime = rFQdqMachiningSecondaryOperation.CycleTime;
                recordToBeUpdated.ManPlusMachineRatePerHour = rFQdqMachiningSecondaryOperation.ManPlusMachineRatePerHour;
                recordToBeUpdated.SecondaryCostPerPart = rFQdqMachiningSecondaryOperation.SecondaryCostPerPart;
                this.DataContext.SaveChanges();
                rFQdqMachiningSecondaryOperation.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("RFQdqMachiningSecondaryOperationSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, rFQdqMachiningSecondaryOperation.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQdqMachiningSecondaryOperationId)
        {
            var RFQdqMachiningSecondaryOperationToBeDeleted = this.DataContext.dqMachiningSecondaryOperations.Where(a => a.Id == RFQdqMachiningSecondaryOperationId).SingleOrDefault();
            if (RFQdqMachiningSecondaryOperationToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQdqMachiningSecondaryOperationNotExists"));
            }
            else
            {
                RFQdqMachiningSecondaryOperationToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqMachiningSecondaryOperationToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqMachiningSecondaryOperationToBeDeleted).State = EntityState.Modified;
                //RFQdqMachiningSecondaryOperationToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQdqMachiningSecondaryOperationDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation>> GetRFQdqMachiningSecondaryOperationList(int RFQSupplierPartDQId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation> lstRFQdqMachiningSecondaryOperation = new List<DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation>();
            DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation rFQdqMachiningSecondaryOperation;
            this.RunOnDB(context =>
             {
                 var rFQdqMachiningSecondaryOperationList = context.dqMachiningSecondaryOperations.Where(c => c.RFQSupplierPartDQId == RFQSupplierPartDQId).ToList();
                 if (rFQdqMachiningSecondaryOperationList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     foreach (var item in rFQdqMachiningSecondaryOperationList)
                     {
                         rFQdqMachiningSecondaryOperation = new DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation();
                         rFQdqMachiningSecondaryOperation.Id = item.Id;
                         rFQdqMachiningSecondaryOperation.RFQSupplierPartDQId = item.RFQSupplierPartDQId;
                         lstRFQdqMachiningSecondaryOperation.Add(rFQdqMachiningSecondaryOperation);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQdqMachiningSecondaryOperation>>(errMSg, lstRFQdqMachiningSecondaryOperation);
            return response;
        }

    }
}
