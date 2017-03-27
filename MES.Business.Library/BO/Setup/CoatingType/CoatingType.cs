using Account.DTO.Library;
using MES.Business.Repositories.Setup.CoatingType;
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

namespace MES.Business.Library.BO.Setup.CoatingType
{
    class CoatingType : ContextBusinessBase, ICoatingTypeRepository
    {
        public CoatingType()
            : base("CoatingType")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.CoatingType.CoatingType coatingType)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.CoatingTypes.AsNoTracking().Any(a => a.CoatingType1 == coatingType.coatingType && a.IsDeleted == false && a.Id != coatingType.Id))
            {
                errMSg = Languages.GetResourceText("CoatingTypeExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.CoatingType();

                if (coatingType.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.CoatingTypes.Where(a => a.Id == coatingType.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("CoatingTypeNotExists");
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
                    this.DataContext.CoatingTypes.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.CoatingType1 = coatingType.coatingType;
                    this.DataContext.SaveChanges();
                    coatingType.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("CoatingTypeSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, coatingType.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.CoatingType.CoatingType> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int coatingTypeId)
        {
            var CoatingTypeToBeDeleted = this.DataContext.CoatingTypes.Where(a => a.Id == coatingTypeId).SingleOrDefault();
            if (CoatingTypeToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("CoatingTypeNotExists"));
            }
            else
            {
                CoatingTypeToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                CoatingTypeToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(CoatingTypeToBeDeleted).State = EntityState.Modified;
                CoatingTypeToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("CoatingTypeDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.CoatingType.CoatingType>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.CoatingType.CoatingType>> GetCoatingTypeList(NPE.Core.IPage<DTO.Library.Setup.CoatingType.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.CoatingType.CoatingType> lstCoatingType = new List<DTO.Library.Setup.CoatingType.CoatingType>();
            DTO.Library.Setup.CoatingType.CoatingType coatingType;
            this.RunOnDB(context =>
             {
                 var CoatingTypeList = context.SearchCoatingType(paging.Criteria.coatingType, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (CoatingTypeList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in CoatingTypeList)
                     {
                         coatingType = new DTO.Library.Setup.CoatingType.CoatingType();
                         coatingType.Id = item.Id;
                         coatingType.coatingType = item.CoatingType;
                         lstCoatingType.Add(coatingType);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.CoatingType.CoatingType>>(errMSg, lstCoatingType);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

    }
}
