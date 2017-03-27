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
    class RFQdqPrimaryProcessConversion : ContextBusinessBase, IRFQdqPrimaryProcessConversionRepository
    {
        public RFQdqPrimaryProcessConversion()
            : base("RFQdqPrimaryProcessConversion")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion rFQdqPrimaryProcessConversion)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.dqPrimaryProcessConversion();

            if (rFQdqPrimaryProcessConversion.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dqPrimaryProcessConversions.Where(a => a.Id == rFQdqPrimaryProcessConversion.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQdqPrimaryProcessConversionNotExists");
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
                this.DataContext.dqPrimaryProcessConversions.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.RFQSupplierPartDQId = rFQdqPrimaryProcessConversion.RFQSupplierPartDQId;
                recordToBeUpdated.MachineDescId = rFQdqPrimaryProcessConversion.MachineDescId;
                if (rFQdqPrimaryProcessConversion.MachineDescId.HasValue)
                {
                    recordToBeUpdated.MachineDescription = this.DataContext.MachineDescs.Where(cm => cm.Id == rFQdqPrimaryProcessConversion.MachineDescId.Value && cm.IsDeleted == false).FirstOrDefault().MachineDescription;
                }
                else
                    recordToBeUpdated.MachineDescription = rFQdqPrimaryProcessConversion.MachineDescription;
                recordToBeUpdated.MachineSize = rFQdqPrimaryProcessConversion.MachineSize;
                recordToBeUpdated.CycleTime = rFQdqPrimaryProcessConversion.CycleTime;
                recordToBeUpdated.ManPlusMachineRatePerHour = rFQdqPrimaryProcessConversion.ManPlusMachineRatePerHour;
                recordToBeUpdated.ProcessConversionCostPerPart = rFQdqPrimaryProcessConversion.ProcessConversionCostPerPart;
                this.DataContext.SaveChanges();
                rFQdqPrimaryProcessConversion.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("RFQdqPrimaryProcessConversionSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, rFQdqPrimaryProcessConversion.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQdqPrimaryProcessConversionId)
        {
            var RFQdqPrimaryProcessConversionToBeDeleted = this.DataContext.dqPrimaryProcessConversions.Where(a => a.Id == RFQdqPrimaryProcessConversionId).SingleOrDefault();
            if (RFQdqPrimaryProcessConversionToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQdqPrimaryProcessConversionNotExists"));
            }
            else
            {
                RFQdqPrimaryProcessConversionToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqPrimaryProcessConversionToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqPrimaryProcessConversionToBeDeleted).State = EntityState.Modified;
                //RFQdqPrimaryProcessConversionToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQdqPrimaryProcessConversionDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the rf QDQ primary process conversion list.
        /// </summary>
        /// <param name="RFQSupplierPartDQId">The RFQ supplier part dq identifier.</param>
        /// <returns></returns>
        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion>> GetRFQdqPrimaryProcessConversionList(int RFQSupplierPartDQId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion> lstRFQdqPrimaryProcessConversion = new List<DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion>();
            DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion rFQdqPrimaryProcessConversion;
            this.RunOnDB(context =>
             {
                 var rFQdqPrimaryProcessConversionList = context.dqPrimaryProcessConversions.Where(c => c.RFQSupplierPartDQId == RFQSupplierPartDQId).ToList();
                 if (rFQdqPrimaryProcessConversionList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     foreach (var item in rFQdqPrimaryProcessConversionList)
                     {
                         rFQdqPrimaryProcessConversion = new DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion();
                         rFQdqPrimaryProcessConversion.Id = item.Id;
                         rFQdqPrimaryProcessConversion.RFQSupplierPartDQId = item.RFQSupplierPartDQId;
                         lstRFQdqPrimaryProcessConversion.Add(rFQdqPrimaryProcessConversion);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQdqPrimaryProcessConversion>>(errMSg, lstRFQdqPrimaryProcessConversion);
            return response;
        }

    }
}
