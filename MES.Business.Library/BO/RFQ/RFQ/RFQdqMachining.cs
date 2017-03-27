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
    class RFQdqMachining : ContextBusinessBase, IRFQdqMachiningRepository
    {
        public RFQdqMachining()
            : base("RFQdqMachining")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQdqMachining rFQdqMachining)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.dqMachining();

            if (rFQdqMachining.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dqMachinings.Where(a => a.Id == rFQdqMachining.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQdqMachiningNotExists");
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
                this.DataContext.dqMachinings.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.RFQSupplierPartDQId = rFQdqMachining.RFQSupplierPartDQId;
                recordToBeUpdated.MachiningDescId = rFQdqMachining.MachiningDescId;
                if (rFQdqMachining.MachiningDescId.HasValue)
                {
                    recordToBeUpdated.MachiningDescription = this.DataContext.MachiningDescs.Where(cm => cm.Id == rFQdqMachining.MachiningDescId.Value && cm.IsDeleted == false).FirstOrDefault().MachiningDescription;
                }
                else
                    recordToBeUpdated.MachiningDescription = rFQdqMachining.MachiningDescription;
                recordToBeUpdated.CycleTime = rFQdqMachining.CycleTime;
                recordToBeUpdated.ManPlusMachineRatePerHour = rFQdqMachining.ManPlusMachineRatePerHour;
                recordToBeUpdated.MachiningCostPerPart = rFQdqMachining.MachiningCostPerPart;
                this.DataContext.SaveChanges();
                rFQdqMachining.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("RFQdqMachiningSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, rFQdqMachining.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQdqMachining> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQdqMachiningId)
        {
            var RFQdqMachiningToBeDeleted = this.DataContext.dqMachinings.Where(a => a.Id == RFQdqMachiningId).SingleOrDefault();
            if (RFQdqMachiningToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQdqMachiningNotExists"));
            }
            else
            {
                RFQdqMachiningToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqMachiningToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqMachiningToBeDeleted).State = EntityState.Modified;
                //RFQdqMachiningToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQdqMachiningDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachining>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqMachining>> GetRFQdqMachiningList(int RFQSupplierPartDQId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQdqMachining> lstRFQdqMachining = new List<DTO.Library.RFQ.RFQ.RFQdqMachining>();
            DTO.Library.RFQ.RFQ.RFQdqMachining rFQdqMachining;
            this.RunOnDB(context =>
             {
                 var rFQdqMachiningList = context.dqMachinings.Where(c => c.RFQSupplierPartDQId == RFQSupplierPartDQId).ToList();
                 if (rFQdqMachiningList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     foreach (var item in rFQdqMachiningList)
                     {
                         rFQdqMachining = new DTO.Library.RFQ.RFQ.RFQdqMachining();
                         rFQdqMachining.Id = item.Id;
                         rFQdqMachining.RFQSupplierPartDQId = item.RFQSupplierPartDQId;
                         lstRFQdqMachining.Add(rFQdqMachining);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQdqMachining>>(errMSg, lstRFQdqMachining);
            return response;
        }

    }
}
