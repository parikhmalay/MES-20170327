using Account.DTO.Library;
using MES.Business.Repositories.Setup.MachineDescription;
using NPE.Core;
using NPE.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core.Extensions;
using System.Data.Entity.Core.Objects;

namespace MES.Business.Library.BO.Setup.MachineDescription
{
    class MachineDescription : ContextBusinessBase, IMachineDescriptionRepository
    {
        public MachineDescription()
            : base("MachineDescription")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.MachineDescription.MachineDescription machineDescription)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.MachineDescs.AsNoTracking().Any(a => a.MachineDescription == machineDescription.machineDescription && a.IsDeleted == false && a.Id != machineDescription.Id))
            {
                errMSg = Languages.GetResourceText("MachineDescriptionExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.MachineDesc();

                if (machineDescription.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.MachineDescs.Where(a => a.Id == machineDescription.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("MachineDescriptionNotExists");
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
                    this.DataContext.MachineDescs.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.MachineDescription = machineDescription.machineDescription;
                    this.DataContext.SaveChanges();
                    machineDescription.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("MachineDescriptionSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, machineDescription.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.MachineDescription.MachineDescription> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int machineDescriptionId)
        {
            var MachineDescriptionToBeDeleted = this.DataContext.MachineDescs.Where(a => a.Id == machineDescriptionId).SingleOrDefault();
            if (MachineDescriptionToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("MachineDescriptionNotExists"));
            }
            else
            {
                MachineDescriptionToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                MachineDescriptionToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(MachineDescriptionToBeDeleted).State = EntityState.Modified;
                MachineDescriptionToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("MachineDescriptionDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.MachineDescription.MachineDescription>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.MachineDescription.MachineDescription>> GetMachineDescriptionList(NPE.Core.IPage<DTO.Library.Setup.MachineDescription.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.MachineDescription.MachineDescription> lstMachineDescription = new List<DTO.Library.Setup.MachineDescription.MachineDescription>();
            DTO.Library.Setup.MachineDescription.MachineDescription machineDescription;
            this.RunOnDB(context =>
             {
                 var MachineDescriptionList = context.SearchMachineDesc(paging.Criteria.machineDescription, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (MachineDescriptionList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in MachineDescriptionList)
                     {
                         machineDescription = new DTO.Library.Setup.MachineDescription.MachineDescription();
                         machineDescription.Id = item.Id;
                         machineDescription.machineDescription = item.MachineDescription;
                         lstMachineDescription.Add(machineDescription);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.MachineDescription.MachineDescription>>(errMSg, lstMachineDescription);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

    }
}
