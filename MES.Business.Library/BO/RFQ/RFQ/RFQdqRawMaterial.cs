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
    class RFQdqRawMaterial : ContextBusinessBase, IRFQdqRawMaterialRepository
    {
        public RFQdqRawMaterial()
            : base("RFQdqRawMaterial")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQdqRawMaterial rFQdqRawMaterial)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.dqRawMaterial();

            if (rFQdqRawMaterial.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dqRawMaterials.Where(a => a.Id == rFQdqRawMaterial.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQdqRawMaterialNotExists");
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
                this.DataContext.dqRawMaterials.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.RFQSupplierPartDQId = rFQdqRawMaterial.RFQSupplierPartDQId;
                recordToBeUpdated.RawMaterialDesc = rFQdqRawMaterial.RawMaterialDesc;
                recordToBeUpdated.RawMatInputInKg = rFQdqRawMaterial.RawMatInputInKg;
                recordToBeUpdated.RawMatCostPerKg = rFQdqRawMaterial.RawMatCostPerKg;
                recordToBeUpdated.RawMatTotal = rFQdqRawMaterial.RawMatTotal;
                recordToBeUpdated.MfgRejectRate = rFQdqRawMaterial.MfgRejectRate;
                recordToBeUpdated.MaterialLoss = rFQdqRawMaterial.MaterialLoss;
                recordToBeUpdated.RawMaterialIndexUsed = rFQdqRawMaterial.RawMaterialIndexUsed;
                this.DataContext.SaveChanges();
                rFQdqRawMaterial.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("RFQdqRawMaterialSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, rFQdqRawMaterial.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQdqRawMaterial> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQdqRawMaterialId)
        {
            var RFQdqRawMaterialToBeDeleted = this.DataContext.dqRawMaterials.Where(a => a.Id == RFQdqRawMaterialId).SingleOrDefault();
            if (RFQdqRawMaterialToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQdqRawMaterialNotExists"));
            }
            else
            {
                RFQdqRawMaterialToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqRawMaterialToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqRawMaterialToBeDeleted).State = EntityState.Modified;
                //RFQdqRawMaterialToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQdqRawMaterialDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqRawMaterial>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqRawMaterial>> GetRFQdqRawMaterialList(int RFQSupplierPartDQId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQdqRawMaterial> lstRFQdqRawMaterial = new List<DTO.Library.RFQ.RFQ.RFQdqRawMaterial>();
            DTO.Library.RFQ.RFQ.RFQdqRawMaterial rFQdqRawMaterial;
            this.RunOnDB(context =>
             {
                 var rFQdqRawMaterialList = context.dqRawMaterials.Where(c => c.RFQSupplierPartDQId == RFQSupplierPartDQId).ToList();
                 if (rFQdqRawMaterialList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     foreach (var item in rFQdqRawMaterialList)
                     {
                         rFQdqRawMaterial = new DTO.Library.RFQ.RFQ.RFQdqRawMaterial();
                         rFQdqRawMaterial.Id = item.Id;
                         rFQdqRawMaterial.RFQSupplierPartDQId = item.RFQSupplierPartDQId;
                         rFQdqRawMaterial.RawMaterialDesc = item.RawMaterialDesc;
                         rFQdqRawMaterial.RawMatInputInKg = item.RawMatInputInKg;
                         rFQdqRawMaterial.RawMatCostPerKg = item.RawMatCostPerKg;
                         rFQdqRawMaterial.RawMatTotal = item.RawMatTotal;
                         rFQdqRawMaterial.MfgRejectRate = item.MfgRejectRate;
                         rFQdqRawMaterial.MaterialLoss = item.MaterialLoss;
                         rFQdqRawMaterial.RawMaterialIndexUsed = item.RawMaterialIndexUsed;
                         lstRFQdqRawMaterial.Add(rFQdqRawMaterial);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQdqRawMaterial>>(errMSg, lstRFQdqRawMaterial);
            return response;
        }

    }
}
