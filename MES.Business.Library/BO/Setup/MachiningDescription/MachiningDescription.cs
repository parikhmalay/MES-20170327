using Account.DTO.Library;
using MES.Business.Repositories.Setup.MachiningDescription;
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

namespace MES.Business.Library.BO.Setup.MachiningDescription
{
    class MachiningDescription : ContextBusinessBase, IMachiningDescriptionRepository
    {
        public MachiningDescription()
            : base("MachiningDescription")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.MachiningDescription.MachiningDescription machiningDescription)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.MachiningDescs.AsNoTracking().Any(a => a.MachiningDescription == machiningDescription.machiningDescription && a.IsDeleted == false && a.Id != machiningDescription.Id))
            {
                errMSg = Languages.GetResourceText("MachiningDescriptionExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.MachiningDesc();

                if (machiningDescription.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.MachiningDescs.Where(a => a.Id == machiningDescription.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("MachiningDescriptionNotExists");
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
                    this.DataContext.MachiningDescs.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.MachiningDescription = machiningDescription.machiningDescription;
                    this.DataContext.SaveChanges();
                    machiningDescription.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("MachiningDescriptionSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, machiningDescription.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.MachiningDescription.MachiningDescription> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int machiningDescriptionId)
        {
            var MachiningDescriptionToBeDeleted = this.DataContext.MachiningDescs.Where(a => a.Id == machiningDescriptionId).SingleOrDefault();
            if (MachiningDescriptionToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("MachiningDescriptionNotExists"));
            }
            else
            {
                MachiningDescriptionToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                MachiningDescriptionToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(MachiningDescriptionToBeDeleted).State = EntityState.Modified;
                MachiningDescriptionToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("MachiningDescriptionDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.MachiningDescription.MachiningDescription>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.MachiningDescription.MachiningDescription>> GetMachiningDescriptionList(NPE.Core.IPage<DTO.Library.Setup.MachiningDescription.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.MachiningDescription.MachiningDescription> lstMachiningDescription = new List<DTO.Library.Setup.MachiningDescription.MachiningDescription>();
            DTO.Library.Setup.MachiningDescription.MachiningDescription machiningDescription;
            this.RunOnDB(context =>
             {
                 var MachiningDescriptionList = context.SearchMachiningDesc(paging.Criteria.machiningDescription, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (MachiningDescriptionList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in MachiningDescriptionList)
                     {
                         machiningDescription = new DTO.Library.Setup.MachiningDescription.MachiningDescription();
                         machiningDescription.Id = item.Id;
                         machiningDescription.machiningDescription = item.MachiningDescription;
                         lstMachiningDescription.Add(machiningDescription);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.MachiningDescription.MachiningDescription>>(errMSg, lstMachiningDescription);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

    }
}
