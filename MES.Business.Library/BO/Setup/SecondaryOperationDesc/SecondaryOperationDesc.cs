using Account.DTO.Library;
using MES.Business.Repositories.Setup.SecondaryOperationDesc;
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

namespace MES.Business.Library.BO.Setup.SecondaryOperationDesc
{
    class SecondaryOperationDesc : ContextBusinessBase,ISecondaryOperationDescRepository
    {
        public SecondaryOperationDesc() : base("SecondaryOperationDesc") { }

        public ITypedResponse<bool?> Delete(int id)
        {
            var secondaryOperationDescToBeDeleted = this.DataContext.SecondaryOperationDescs.Where(a => a.Id == id).SingleOrDefault();
            if (secondaryOperationDescToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("SecondaryOperationDescNotExists"));
            }
            else
            {
                secondaryOperationDescToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                secondaryOperationDescToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(secondaryOperationDescToBeDeleted).State = EntityState.Modified;
                secondaryOperationDescToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("SecondaryOperationDescDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc secondaryOperationDesc)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.SecondaryOperationDescs.AsNoTracking().Any(a => a.SecondaryOperationDescription == secondaryOperationDesc.secondaryOperationDesc && a.IsDeleted == false && a.Id != secondaryOperationDesc.Id))
            {
                errMSg = Languages.GetResourceText("SecondaryOperationDescExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.SecondaryOperationDesc();

                if (secondaryOperationDesc.HasId())
                {
                    recordToBeUpdated = this.DataContext.SecondaryOperationDescs.Where(a => a.Id == secondaryOperationDesc.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("SecondaryOperationDescNotExists");
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
                    this.DataContext.SecondaryOperationDescs.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.SecondaryOperationDescription = secondaryOperationDesc.secondaryOperationDesc;
                    this.DataContext.SaveChanges();
                    secondaryOperationDesc.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("SecondaryOperationDescSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, secondaryOperationDesc.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>> GetSecondaryOperationDescs(IPage<DTO.Library.Setup.SecondaryOperationDesc.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = null;

            //declare paging variables
            /*int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            int RecordStart = PageNumber * PageSize;*/

            List<DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc> secondaryOperationDescs = new List<DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>();
            DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc secondaryOperationDesc;
            this.RunOnDB(context =>
            {
                var secondaryOperationDescList = context.SearchSecondaryOperationDescs(paging.Criteria.secondaryOperationDesc, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (secondaryOperationDescList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in secondaryOperationDescList)
                    {
                        secondaryOperationDesc = new DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc();
                        secondaryOperationDesc.Id = item.Id;
                        secondaryOperationDesc.secondaryOperationDesc = item.SecondaryOperationDescription;
                        secondaryOperationDescs.Add(secondaryOperationDesc);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>>(errMSg, secondaryOperationDescs);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
