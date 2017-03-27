using Account.DTO.Library;
using MES.Business.Repositories.Setup.ProjectStage;
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

namespace MES.Business.Library.BO.Setup.ProjectStage
{
    class ProjectStage : ContextBusinessBase, IProjectStageRepository
    {
        public ProjectStage()
            : base("ProjectStage")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.ProjectStage.ProjectStage projectStage)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.ProjectStages.AsNoTracking().Any(a => a.ProjectStage1 == projectStage.projectStage && a.IsDeleted == false && a.Id != projectStage.Id))
            {
                errMSg = Languages.GetResourceText("ProjectStageExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.ProjectStage();

                if (projectStage.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.ProjectStages.Where(a => a.Id == projectStage.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("ProjectStageNotExists");
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
                    this.DataContext.ProjectStages.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.ProjectStage1 = projectStage.projectStage;
                    recordToBeUpdated.ProjectCategoryId = projectStage.ProjectCategoryId;
                    this.DataContext.SaveChanges();
                    projectStage.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("ProjectStageSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, projectStage.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.ProjectStage.ProjectStage> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int projectStageId)
        {
            var projectStageToBeDeleted = this.DataContext.ProjectStages.Where(a => a.Id == projectStageId).SingleOrDefault();
            if (projectStageToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("ProjectStageNotExists"));
            }
            else
            {
                projectStageToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                projectStageToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(projectStageToBeDeleted).State = EntityState.Modified;
                projectStageToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("ProjectStageDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.ProjectStage.ProjectStage>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.ProjectStage.ProjectStage>> GetProjectStagesList(NPE.Core.IPage<DTO.Library.Setup.ProjectStage.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.ProjectStage.ProjectStage> lstprojectStage = new List<DTO.Library.Setup.ProjectStage.ProjectStage>();
            DTO.Library.Setup.ProjectStage.ProjectStage projectStage;
            this.RunOnDB(context =>
             {
                 var projectStageList = context.SearchProjectStages(paging.Criteria.projectStage, paging.Criteria.ProjectCategoryId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (projectStageList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in projectStageList)
                     {
                         projectStage = new DTO.Library.Setup.ProjectStage.ProjectStage();
                         projectStage.Id = item.Id;
                         projectStage.projectStage = item.ProjectStage;
                         projectStage.ProjectCategoryId = item.ProjectCategoryId;
                         projectStage.ProjectCategory = item.ProjectCategory;
                         lstprojectStage.Add(projectStage);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.ProjectStage.ProjectStage>>(errMSg, lstprojectStage);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
