using Account.DTO.Library;
using MES.Business.Repositories.Setup.Status;
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

namespace MES.Business.Library.BO.Setup.Status
{
    class Status : ContextBusinessBase, IStatusRepository
    {
        public Status() : base("Status") { }

        public ITypedResponse<bool?> Delete(int id)
        {
            var statusToBeDeleted = this.DataContext.Status.Where(a => a.Id == id).SingleOrDefault();
            if (statusToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("StatusNotExists"));
            }
            else
            {
                statusToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                statusToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(statusToBeDeleted).State = EntityState.Modified;
                statusToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("StatusDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.Status.Status> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.Status.Status status)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Status.AsNoTracking().Any(a => a.Status1 == status.status && a.IsDeleted == false && a.Id != status.Id))
            {
                errMSg = Languages.GetResourceText("StatusExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Status();

                if (status.HasId())
                {
                    recordToBeUpdated = this.DataContext.Status.Where(a => a.Id == status.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("StatusNotExists");
                    else
                    {
                        /*Delete All StatusAssociatedTo*/
                        var deleteStatusAssociatedToList = this.DataContext.StatusAssociatedToes.Where(a => a.StatusId == status.Id).ToList();
                        foreach (var item in deleteStatusAssociatedToList)
                        {
                            this.DataContext.StatusAssociatedToes.Remove(item);
                        }

                        recordToBeUpdated.UpdatedDate = AuditUtils.GetCurrentDateTime();
                        recordToBeUpdated.UpdatedBy = CurrentUser;
                        this.DataContext.Entry(recordToBeUpdated).State = EntityState.Modified;
                    }
                }
                else
                {
                    recordToBeUpdated.CreatedBy = recordToBeUpdated.UpdatedBy = CurrentUser;
                    recordToBeUpdated.CreatedDate = AuditUtils.GetCurrentDateTime();
                    this.DataContext.Status.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.Status1 = status.status;
                    this.DataContext.SaveChanges();
                    status.Id = recordToBeUpdated.Id;

                    /*Insert StatusAssociatedTo*/
                    MES.Data.Library.StatusAssociatedTo dboStatusAssociatedTo = null;
                    if (status.StatusAssociatedToList != null && status.StatusAssociatedToList.Count > 0)
                    {
                        bool AnyRemarksAssociatedTo = false;
                        foreach (var remarksAssociatedTo in status.StatusAssociatedToList)
                        {
                            if (remarksAssociatedTo.Id != 0)
                            {
                                AnyRemarksAssociatedTo = true;
                                dboStatusAssociatedTo = new MES.Data.Library.StatusAssociatedTo();
                                dboStatusAssociatedTo.StatusId = status.Id;
                                dboStatusAssociatedTo.AssociatedToId = remarksAssociatedTo.Id;
                                dboStatusAssociatedTo.CreatedBy = CurrentUser;
                                dboStatusAssociatedTo.CreatedDate = AuditUtils.GetCurrentDateTime();
                                this.DataContext.StatusAssociatedToes.Add(dboStatusAssociatedTo);
                            }
                        }
                        if (AnyRemarksAssociatedTo)
                            this.DataContext.SaveChanges();
                    }

                    successMsg = Languages.GetResourceText("StatuseSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, status.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.Status.Status>> GetStatus(IPage<DTO.Library.Setup.Status.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.Status.Status> statuss = new List<DTO.Library.Setup.Status.Status>();
            DTO.Library.Setup.Status.Status status;
            this.RunOnDB(context =>
            {
                var statusList = context.SearchStatus(paging.Criteria.status, paging.Criteria.AssociatedToId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (statusList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in statusList)
                    {
                        status = new DTO.Library.Setup.Status.Status();
                        status.Id = item.Id;
                        status.status = item.Status;
                        /*Get StatusAssociatedTo List*/
                        status.StatusAssociatedToList = new List<DTO.Library.Setup.Status.StatusAssociatedTo>();
                        context.StatusAssociatedToes.Where(a => a.StatusId == item.Id).ToList().ForEach(tl => status.StatusAssociatedToList.Add(
                            new DTO.Library.Setup.Status.StatusAssociatedTo()
                            {
                                Id = Convert.ToInt32(tl.AssociatedToId),
                                Name = tl.AssociatedTo.AssociatedTo1,
                                StatusId = Convert.ToInt32(tl.StatusId),
                                AssociatedToId = Convert.ToInt32(tl.AssociatedToId),
                                AssociatedTo = tl.AssociatedTo.AssociatedTo1,
                                Description = tl.AssociatedTo.Description
                            }));
                        statuss.Add(status);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Status.Status>>(errMSg, statuss);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.Status.Status>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
