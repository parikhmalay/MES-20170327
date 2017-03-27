using Account.DTO.Library;
using MES.Business.Repositories.Setup.RFQPriority;
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

namespace MES.Business.Library.BO.Setup.RFQPriority
{
    class RFQPriority : ContextBusinessBase, IRFQPriorityRepository
    {
        public RFQPriority() : base("RFQPriority") { }

        public ITypedResponse<bool?> Delete(int id)
        {
            var RFQPriorityToBeDeleted = this.DataContext.RFQPriorities.Where(a => a.Id == id).SingleOrDefault();
            if (RFQPriorityToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RFQPriorityNotExists"));
            }
            else
            {
                RFQPriorityToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                RFQPriorityToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(RFQPriorityToBeDeleted).State = EntityState.Modified;
                RFQPriorityToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQPriorityDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.RFQPriority.RFQPriority> FindById(int id)
        {
            string errMSg = string.Empty;

            DTO.Library.Setup.RFQPriority.RFQPriority RFQPriority = new DTO.Library.Setup.RFQPriority.RFQPriority();

            var RFQPriorityItem = this.DataContext.RFQPriorities.Where(a => a.Id == id).SingleOrDefault();
            if (RFQPriorityItem == null)
                errMSg = Languages.GetResourceText("RFQPriorityNotExists");
            else
            {
                #region general details
                RFQPriority = ObjectLibExtensions.AutoConvert<DTO.Library.Setup.RFQPriority.RFQPriority>(RFQPriorityItem);
                #endregion
            }
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.RFQPriority.RFQPriority>(errMSg, RFQPriority);
            return response;
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.RFQPriority.RFQPriority RFQPriority)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.RFQPriorities.AsNoTracking().Any(a => a.rfqPriority1 == RFQPriority.rfqPriority && a.IsDeleted == false && a.Id != RFQPriority.Id))
            {
                errMSg = Languages.GetResourceText("RFQPriorityExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.RFQPriority();

                if (RFQPriority.HasId())
                {
                    recordToBeUpdated = this.DataContext.RFQPriorities.Where(a => a.Id == RFQPriority.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("RFQPriorityNotExists");
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
                    this.DataContext.RFQPriorities.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.rfqPriority1 = RFQPriority.rfqPriority;
                    this.DataContext.SaveChanges();
                    RFQPriority.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("RFQPriorityeSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, RFQPriority.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.RFQPriority.RFQPriority>> GetRFQPrioritys(IPage<DTO.Library.Setup.RFQPriority.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = null;

            //declare paging variables
            /*int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            int RecordStart = PageNumber * PageSize;*/

            List<DTO.Library.Setup.RFQPriority.RFQPriority> RFQPrioritys = new List<DTO.Library.Setup.RFQPriority.RFQPriority>();
            DTO.Library.Setup.RFQPriority.RFQPriority RFQPriority;
            this.RunOnDB(context =>
            {
                var RFQPriorityList = context.SearchRFQPrioritys(paging.Criteria.RFQPriority, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (RFQPriorityList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in RFQPriorityList)
                    {
                        RFQPriority = new DTO.Library.Setup.RFQPriority.RFQPriority();
                        RFQPriority.Id = item.Id;
                        RFQPriority.rfqPriority = item.RFQPriority;
                        RFQPrioritys.Add(RFQPriority);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.RFQPriority.RFQPriority>>(errMSg, RFQPrioritys);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.RFQPriority.RFQPriority>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
