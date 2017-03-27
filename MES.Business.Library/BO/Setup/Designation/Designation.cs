using Account.DTO.Library;
using MES.Business.Repositories.Setup.Designation;
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

namespace MES.Business.Library.BO.Setup.Designation
{
    class Designation : ContextBusinessBase, IDesignationRepository
    {
        public Designation()
            : base("Designation")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.Designation.Designation designation)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Designations.AsNoTracking().Any(a => a.Designation1 == designation.designation && a.IsDeleted == false && a.Id != designation.Id))
            {
                errMSg = Languages.GetResourceText("DesignationExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Designation();

                if (designation.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.Designations.Where(a => a.Id == designation.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("DesignationNotExists");
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
                    this.DataContext.Designations.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.Designation1 = designation.designation;
                    this.DataContext.SaveChanges();
                    designation.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("DesignationSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, designation.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.Designation.Designation> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int designationId)
        {
            var DesignationToBeDeleted = this.DataContext.Designations.Where(a => a.Id == designationId).SingleOrDefault();
            if (DesignationToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("DesignationNotExists"));
            }
            else
            {
                DesignationToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                DesignationToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(DesignationToBeDeleted).State = EntityState.Modified;
                DesignationToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("DesignationDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Designation.Designation>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Designation.Designation>> GetDesignationList(NPE.Core.IPage<DTO.Library.Setup.Designation.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.Designation.Designation> lstDesignation = new List<DTO.Library.Setup.Designation.Designation>();
            DTO.Library.Setup.Designation.Designation designation;
            this.RunOnDB(context =>
             {
                 var DesignationList = context.SearchDesignation(paging.Criteria.designation, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (DesignationList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in DesignationList)
                     {
                         designation = new DTO.Library.Setup.Designation.Designation();
                         designation.Id = item.Id;
                         designation.designation = item.Designation;
                         lstDesignation.Add(designation);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Designation.Designation>>(errMSg, lstDesignation);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
