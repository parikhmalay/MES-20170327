using Account.DTO.Library;
using MES.Business.Repositories.Setup.IndustryType;
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
using MES.Business.Mapping.Extensions;

namespace MES.Business.Library.BO.Setup.IndustryType
{
    class IndustryType : ContextBusinessBase, IIndustryTypeRepository
    {
        public IndustryType() : base("IndustryType") { }

        public ITypedResponse<bool?> Delete(int id)
        {
            var industryTypeToBeDeleted = this.DataContext.IndustryTypes.Where(a => a.Id == id).SingleOrDefault();
            if (industryTypeToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("IndustryTypeNotExists"));
            }
            else
            {
                industryTypeToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                industryTypeToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(industryTypeToBeDeleted).State = EntityState.Modified;
                industryTypeToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("IndustryTypeDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.IndustryType.IndustryType> FindById(int id)
        {
            string errMSg = string.Empty;

            DTO.Library.Setup.IndustryType.IndustryType industryType = new DTO.Library.Setup.IndustryType.IndustryType();

            var industryTypeItem = this.DataContext.IndustryTypes.Where(a => a.Id == id).SingleOrDefault();
            if (industryTypeItem == null)
                errMSg = Languages.GetResourceText("IndustryTypeNotExists");
            else
            {
                #region general details
                industryType = ObjectLibExtensions.AutoConvert<DTO.Library.Setup.IndustryType.IndustryType>(industryTypeItem);
                #endregion
            }
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.IndustryType.IndustryType>(errMSg, industryType);
            return response;
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.IndustryType.IndustryType industryType)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.IndustryTypes.AsNoTracking().Any(a => a.IndustryType1 == industryType.industryType && a.IsDeleted == false && a.Id != industryType.Id))
            {
                errMSg = Languages.GetResourceText("IndustryTypeExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.IndustryType();

                if (industryType.HasId())
                {
                    recordToBeUpdated = this.DataContext.IndustryTypes.Where(a => a.Id == industryType.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("IndustryTypeNotExists");
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
                    this.DataContext.IndustryTypes.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.IndustryType1 = industryType.industryType;
                    this.DataContext.SaveChanges();
                    industryType.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("IndustryTypeSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, industryType.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.IndustryType.IndustryType>> GetIndustryTypes(IPage<DTO.Library.Setup.IndustryType.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = null;

            //declare paging variables
            /*int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            int RecordStart = PageNumber * PageSize;*/

            List<DTO.Library.Setup.IndustryType.IndustryType> industryTypes = new List<DTO.Library.Setup.IndustryType.IndustryType>();
            DTO.Library.Setup.IndustryType.IndustryType industryType;
            this.RunOnDB(context =>
            {
                var industryTypeList = context.SearchIndustryTypes(paging.Criteria.industrytype, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (industryTypeList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in industryTypeList)
                    {
                        industryType = new DTO.Library.Setup.IndustryType.IndustryType();
                        industryType.Id = item.Id;
                        industryType.industryType = item.IndustryType;
                        industryTypes.Add(industryType);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.IndustryType.IndustryType>>(errMSg, industryTypes);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.IndustryType.IndustryType>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
