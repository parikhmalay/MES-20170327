using Account.DTO.Library;
using MES.Business.Repositories.Setup.ProjectCategory;
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

namespace MES.Business.Library.BO.Setup.ProjectCategory
{
    class ProjectCategory : ContextBusinessBase, IProjectCategoryRepository
    {
        public ProjectCategory()
            : base("ProjectCategory")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.ProjectCategory.ProjectCategory projectCategory)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.ProjectCategories.AsNoTracking().Any(a => a.ProjectCategory1 == projectCategory.projectCategory && a.IsDeleted == false && a.Id != projectCategory.Id))
            {
                errMSg = Languages.GetResourceText("ProjectCategoryExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.ProjectCategory();

                if (projectCategory.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.ProjectCategories.Where(a => a.Id == projectCategory.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("ProjectCategoryNotExists");
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
                    this.DataContext.ProjectCategories.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.ProjectCategory1 = projectCategory.projectCategory;
                    this.DataContext.SaveChanges();
                    projectCategory.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("ProjectCategorySavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, projectCategory.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.ProjectCategory.ProjectCategory> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int projectCategoryId)
        {
            if (this.DataContext.ProjectStages.Any(a => a.ProjectCategoryId == projectCategoryId))
            {
                return FailedBoolResponse(Languages.GetResourceText("ProjectCategoryIsInUse"));
            }

            var projectCategoryToBeDeleted = this.DataContext.ProjectCategories.Where(a => a.Id == projectCategoryId).SingleOrDefault();
            if (projectCategoryToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("ProjectCategoryNotExists"));
            }
            else
            {
                projectCategoryToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                projectCategoryToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(projectCategoryToBeDeleted).State = EntityState.Modified;
                projectCategoryToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("ProjectCategoryDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.ProjectCategory.ProjectCategory>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.ProjectCategory.ProjectCategory>> GetProjectCategoryList(NPE.Core.IPage<DTO.Library.Setup.ProjectCategory.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = null;

            //declare paging variables
            //int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            //int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            //int RecordStart = PageNumber * PageSize;

            List<DTO.Library.Setup.ProjectCategory.ProjectCategory> lstProjectCategory = new List<DTO.Library.Setup.ProjectCategory.ProjectCategory>();
            DTO.Library.Setup.ProjectCategory.ProjectCategory projectCategory;
            this.RunOnDB(context =>
             {
                 var projectCategoryList = context.SearchProjectCategory(paging.Criteria.projectCategory, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (projectCategoryList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in projectCategoryList)
                     {
                         projectCategory = new DTO.Library.Setup.ProjectCategory.ProjectCategory();
                         projectCategory.Id = item.Id;
                         projectCategory.projectCategory = item.ProjectCategory;
                         lstProjectCategory.Add(projectCategory);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.ProjectCategory.ProjectCategory>>(errMSg, lstProjectCategory);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

    }
}
