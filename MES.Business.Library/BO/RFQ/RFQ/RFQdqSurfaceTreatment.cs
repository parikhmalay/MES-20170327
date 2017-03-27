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
    class RFQdqSurfaceTreatment : ContextBusinessBase, IRFQdqSurfaceTreatmentRepository
    {
        public RFQdqSurfaceTreatment()
            : base("RFQdqSurfaceTreatment")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment rFQdqSurfaceTreatment)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.dqSurfaceTreatment();

            if (rFQdqSurfaceTreatment.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dqSurfaceTreatments.Where(a => a.Id == rFQdqSurfaceTreatment.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQdqSurfaceTreatmentNotExists");
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
                this.DataContext.dqSurfaceTreatments.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.RFQSupplierPartDQId = rFQdqSurfaceTreatment.RFQSupplierPartDQId;
                recordToBeUpdated.CoatingTypeId = rFQdqSurfaceTreatment.CoatingTypeId;
                if (rFQdqSurfaceTreatment.CoatingTypeId.HasValue)
                {
                    recordToBeUpdated.CoatingType = this.DataContext.CoatingTypes.Where(cm => cm.Id == rFQdqSurfaceTreatment.CoatingTypeId.Value && cm.IsDeleted == false).FirstOrDefault().CoatingType1;
                }
                else
                    recordToBeUpdated.CoatingType = rFQdqSurfaceTreatment.CoatingType;
                recordToBeUpdated.PartsPerHour = rFQdqSurfaceTreatment.PartsPerHour;
                recordToBeUpdated.ManPlusMachineRatePerHour = rFQdqSurfaceTreatment.ManPlusMachineRatePerHour;
                recordToBeUpdated.CoatingCostPerHour = rFQdqSurfaceTreatment.CoatingCostPerHour;
                this.DataContext.SaveChanges();
                rFQdqSurfaceTreatment.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("RFQdqSurfaceTreatmentSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, rFQdqSurfaceTreatment.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQdqSurfaceTreatmentId)
        {
            var RFQdqSurfaceTreatmentToBeDeleted = this.DataContext.dqSurfaceTreatments.Where(a => a.Id == RFQdqSurfaceTreatmentId).SingleOrDefault();
            if (RFQdqSurfaceTreatmentToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQdqSurfaceTreatmentNotExists"));
            }
            else
            {
                RFQdqSurfaceTreatmentToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqSurfaceTreatmentToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqSurfaceTreatmentToBeDeleted).State = EntityState.Modified;
                //RFQdqSurfaceTreatmentToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQdqSurfaceTreatmentDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment>> GetRFQdqSurfaceTreatmentList(int RFQSupplierPartDQId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment> lstRFQdqSurfaceTreatment = new List<DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment>();
            DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment rFQdqSurfaceTreatment;
            this.RunOnDB(context =>
             {
                 var rFQdqSurfaceTreatmentList = context.dqSurfaceTreatments.Where(c => c.RFQSupplierPartDQId == RFQSupplierPartDQId).ToList();
                 if (rFQdqSurfaceTreatmentList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     foreach (var item in rFQdqSurfaceTreatmentList)
                     {
                         rFQdqSurfaceTreatment = new DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment();
                         rFQdqSurfaceTreatment.Id = item.Id;
                         rFQdqSurfaceTreatment.RFQSupplierPartDQId = item.RFQSupplierPartDQId;
                         lstRFQdqSurfaceTreatment.Add(rFQdqSurfaceTreatment);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQdqSurfaceTreatment>>(errMSg, lstRFQdqSurfaceTreatment);
            return response;
        }

    }
}
