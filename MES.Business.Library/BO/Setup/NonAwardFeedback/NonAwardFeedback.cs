using Account.DTO.Library;
using MES.Business.Repositories.Setup.NonAwardFeedback;
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

namespace MES.Business.Library.BO.Setup.NonAwardFeedback
{
    class NonAwardFeedback : ContextBusinessBase,INonAwardFeedbackRepository
    {
        public NonAwardFeedback() : base("NonAwardFeedback") { }

        public ITypedResponse<bool?> Delete(int id)
        {
            var nonAwardFeedbackToBeDeleted = this.DataContext.NonAwardFeedbacks.Where(a => a.Id == id).SingleOrDefault();
            if (nonAwardFeedbackToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("NonAwardFeedbackNotExists"));
            }
            else
            {
                nonAwardFeedbackToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                nonAwardFeedbackToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(nonAwardFeedbackToBeDeleted).State = EntityState.Modified;
                nonAwardFeedbackToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("NonAwardFeedbackDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback nonAwardFeedback)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.NonAwardFeedbacks.AsNoTracking().Any(a => a.NonAwardFeedback1 == nonAwardFeedback.nonAwardFeedback && a.IsDeleted == false && a.Id != nonAwardFeedback.Id))
            {
                errMSg = Languages.GetResourceText("NonAwardFeedbackExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.NonAwardFeedback();

                if (nonAwardFeedback.HasId())
                {
                    recordToBeUpdated = this.DataContext.NonAwardFeedbacks.Where(a => a.Id == nonAwardFeedback.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("NonAwardFeedbackNotExists");
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
                    this.DataContext.NonAwardFeedbacks.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.NonAwardFeedback1 = nonAwardFeedback.nonAwardFeedback;
                    this.DataContext.SaveChanges();
                    nonAwardFeedback.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("NonAwardFeedbackeSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, nonAwardFeedback.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>> GetNonAwardFeedbacks(IPage<DTO.Library.Setup.NonAwardFeedback.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = null;

            //declare paging variables
            /*int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            int RecordStart = PageNumber * PageSize;*/

            List<DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback> nonAwardFeedbacks = new List<DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>();
            DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback nonAwardFeedback;
            this.RunOnDB(context =>
            {
                var nonAwardFeedbackList = context.SearchNonAwardFeedbacks(paging.Criteria.nonAwardFeedback, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (nonAwardFeedbackList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in nonAwardFeedbackList)
                    {
                        nonAwardFeedback = new DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback();
                        nonAwardFeedback.Id = item.Id;
                        nonAwardFeedback.nonAwardFeedback = item.NonAwardFeedback;
                        nonAwardFeedbacks.Add(nonAwardFeedback);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>>(errMSg, nonAwardFeedbacks);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
