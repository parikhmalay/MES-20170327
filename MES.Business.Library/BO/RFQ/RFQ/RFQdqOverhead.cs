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
    class RFQdqOverhead : ContextBusinessBase, IRFQdqOverheadRepository
    {
        public RFQdqOverhead()
            : base("RFQdqOverhead")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.RFQ.RFQdqOverhead rFQdqOverhead)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.dqOverhead();

            if (rFQdqOverhead.Id > 0)
            {
                recordToBeUpdated = this.DataContext.dqOverheads.Where(a => a.Id == rFQdqOverhead.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("RFQdqOverheadNotExists");
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
                this.DataContext.dqOverheads.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.RFQSupplierPartDQId = rFQdqOverhead.RFQSupplierPartDQId;
                recordToBeUpdated.InventoryCarryingCost = rFQdqOverhead.InventoryCarryingCost;
                recordToBeUpdated.Packing = rFQdqOverhead.Packing;
                recordToBeUpdated.LocalFreightToPort = rFQdqOverhead.LocalFreightToPort;
                recordToBeUpdated.ProfitAndSGA = rFQdqOverhead.ProfitAndSGA;
                recordToBeUpdated.OverheadPercentPiecePrice = rFQdqOverhead.OverheadPercentPiecePrice;
                recordToBeUpdated.PackagingMaterial = rFQdqOverhead.PackagingMaterial;
                this.DataContext.SaveChanges();
                rFQdqOverhead.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("RFQdqOverheadSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, rFQdqOverhead.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.RFQ.RFQdqOverhead> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int RFQdqOverheadId)
        {
            var RFQdqOverheadToBeDeleted = this.DataContext.dqOverheads.Where(a => a.Id == RFQdqOverheadId).SingleOrDefault();
            if (RFQdqOverheadToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQdqOverheadNotExists"));
            }
            else
            {
                RFQdqOverheadToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQdqOverheadToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQdqOverheadToBeDeleted).State = EntityState.Modified;
                //RFQdqOverheadToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQdqOverheadDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqOverhead>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQdqOverhead>> GetRFQdqOverheadList(int RFQSupplierPartDQId)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.RFQ.RFQdqOverhead> lstRFQdqOverhead = new List<DTO.Library.RFQ.RFQ.RFQdqOverhead>();
            DTO.Library.RFQ.RFQ.RFQdqOverhead rFQdqOverhead;
            this.RunOnDB(context =>
             {
                 var rFQdqOverheadList = context.dqOverheads.Where(c => c.RFQSupplierPartDQId == RFQSupplierPartDQId).ToList();
                 if (rFQdqOverheadList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     foreach (var item in rFQdqOverheadList)
                     {
                         rFQdqOverhead = new DTO.Library.RFQ.RFQ.RFQdqOverhead();
                         rFQdqOverhead.Id = item.Id;
                         rFQdqOverhead.RFQSupplierPartDQId = item.RFQSupplierPartDQId;
                         lstRFQdqOverhead.Add(rFQdqOverhead);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQdqOverhead>>(errMSg, lstRFQdqOverhead);
            return response;
        }

    }
}
