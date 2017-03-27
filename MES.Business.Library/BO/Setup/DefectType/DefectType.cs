using Account.DTO.Library;
using MES.Business.Repositories.Setup.DefectType;
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

namespace MES.Business.Library.BO.Setup.DefectType
{
    class DefectType : ContextBusinessBase, IDefectTypeRepository
    {
        public DefectType()
            : base("DefectType")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.DefectType.DefectType defectType)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.DefectTypes.AsNoTracking().Any(a => a.DefectType1 == defectType.defectType && a.IsDeleted == false && a.Id != defectType.Id))
            {
                errMSg = Languages.GetResourceText("DefectTypeExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.DefectType();

                if (defectType.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.DefectTypes.Where(a => a.Id == defectType.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("DefectTypeNotExists");
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
                    this.DataContext.DefectTypes.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.DefectType1 = defectType.defectType;
                    this.DataContext.SaveChanges();
                    defectType.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("DefectTypeSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, defectType.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.DefectType.DefectType> FindById(int id)
        {
            string errMSg = string.Empty;

            DTO.Library.Setup.DefectType.DefectType defectType = new DTO.Library.Setup.DefectType.DefectType();
            this.RunOnDB(context =>
            {
                var item = context.DefectTypes.Where(d => d.Id == id).SingleOrDefault();
                if (item == null)
                    errMSg = Languages.GetResourceText("DefectTypeNotExists");
                else
                {
                    defectType.Id = item.Id;
                    defectType.defectType = item.DefectType1;
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.DefectType.DefectType>(errMSg, defectType);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int defectTypeId)
        {
            var DefectTypeToBeDeleted = this.DataContext.DefectTypes.Where(a => a.Id == defectTypeId).SingleOrDefault();
            if (DefectTypeToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("DefectTypeNotExists"));
            }
            else
            {
                DefectTypeToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                DefectTypeToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(DefectTypeToBeDeleted).State = EntityState.Modified;
                DefectTypeToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("DefectTypeDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.DefectType.DefectType>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.DefectType.DefectType>> GetDefectTypeList(NPE.Core.IPage<DTO.Library.Setup.DefectType.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.DefectType.DefectType> lstDefectType = new List<DTO.Library.Setup.DefectType.DefectType>();
            DTO.Library.Setup.DefectType.DefectType defectType;
            this.RunOnDB(context =>
             {
                 var DefectTypeList = context.SearchDefectType(paging.Criteria.defectType, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (DefectTypeList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in DefectTypeList)
                     {
                         defectType = new DTO.Library.Setup.DefectType.DefectType();
                         defectType.Id = item.Id;
                         defectType.defectType = item.DefectType;
                         lstDefectType.Add(defectType);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.DefectType.DefectType>>(errMSg, lstDefectType);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

    }
}
