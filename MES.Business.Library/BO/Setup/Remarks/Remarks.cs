using Account.DTO.Library;
using MES.Business.Repositories.Setup.Remarks;
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


namespace MES.Business.Library.BO.Setup.Remarks
{
    class Remarks : ContextBusinessBase, IRemarksRepository
    {
        public Remarks() : base("Remarks") { }


        public ITypedResponse<bool?> Delete(int id)
        {
            var remarksToBeDeleted = this.DataContext.Remarks.Where(a => a.Id == id).SingleOrDefault();
            if (remarksToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("RemarksNotExists"));
            }
            else
            {
                remarksToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                remarksToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(remarksToBeDeleted).State = EntityState.Modified;
                remarksToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RemarksDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.Remarks.Remarks> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.Remarks.Remarks remarks)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Remarks.AsNoTracking().Any(a => a.Remarks == remarks.remarks && a.IsDeleted == false && a.Id != remarks.Id))
            {
                errMSg = Languages.GetResourceText("RemarksExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Remark();

                if (remarks.HasId())
                {
                    recordToBeUpdated = this.DataContext.Remarks.Where(a => a.Id == remarks.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("RemarksNotExists");
                    else
                    {
                        /*Delete All StatusAssociatedTo*/
                        var deleteRemarksAssociatedToList = this.DataContext.RemarksAssociatedToes.Where(a => a.RemarksId == remarks.Id).ToList();
                        foreach (var item in deleteRemarksAssociatedToList)
                        {
                            this.DataContext.RemarksAssociatedToes.Remove(item);
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
                    this.DataContext.Remarks.Add(recordToBeUpdated);
                }

                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.Remarks = remarks.remarks;
                    this.DataContext.SaveChanges();
                    remarks.Id = recordToBeUpdated.Id;

                    /*Insert RemarksAssociatedTo*/
                    MES.Data.Library.RemarksAssociatedTo dboRemarksAssociatedTo = null;
                    if (remarks.RemarksAssociatedToList != null && remarks.RemarksAssociatedToList.Count > 0)
                    {
                        bool AnyRemarksAssociatedTo = false;
                        foreach (var remarksAssociatedTo in remarks.RemarksAssociatedToList)
                        {
                            if (remarksAssociatedTo.Id != 0)
                            {
                                AnyRemarksAssociatedTo = true;
                                dboRemarksAssociatedTo = new MES.Data.Library.RemarksAssociatedTo();
                                dboRemarksAssociatedTo.RemarksId = remarks.Id;
                                dboRemarksAssociatedTo.AssociatedToId = remarksAssociatedTo.Id;
                                dboRemarksAssociatedTo.CreatedBy = CurrentUser;
                                dboRemarksAssociatedTo.CreatedDate = AuditUtils.GetCurrentDateTime();
                                this.DataContext.RemarksAssociatedToes.Add(dboRemarksAssociatedTo);
                            }
                        }
                        if (AnyRemarksAssociatedTo)
                            this.DataContext.SaveChanges();
                    }

                    successMsg = Languages.GetResourceText("RemarksSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, remarks.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.Remarks.Remarks>> GetRemarks(IPage<DTO.Library.Setup.Remarks.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.Remarks.Remarks> remarksListing = new List<DTO.Library.Setup.Remarks.Remarks>();
            DTO.Library.Setup.Remarks.Remarks remarks;
            this.RunOnDB(context =>
            {
                var remarksList = context.SearchRemarks(paging.Criteria.remarks, paging.Criteria.AssociatedToId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (remarksList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in remarksList)
                    {
                        remarks = new DTO.Library.Setup.Remarks.Remarks();
                        remarks.Id = item.Id;
                        remarks.remarks = item.Remarks;
                        /*Get RemarksAssociatedTo*/
                        remarks.RemarksAssociatedToList = new List<DTO.Library.Setup.Remarks.RemarksAssociatedTo>();
                        context.RemarksAssociatedToes.Where(a => a.RemarksId == item.Id).ToList().ForEach(tl => remarks.RemarksAssociatedToList.Add(
                            new DTO.Library.Setup.Remarks.RemarksAssociatedTo()
                            {
                                Id = Convert.ToInt32(tl.AssociatedToId),
                                Name = tl.AssociatedTo.AssociatedTo1,
                                RemarksId = Convert.ToInt32(tl.RemarksId),
                                AssociatedToId = Convert.ToInt32(tl.AssociatedToId),
                                AssociatedTo = tl.AssociatedTo.AssociatedTo1,
                                Description = tl.AssociatedTo.Description
                            }));
                        remarksListing.Add(remarks);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Remarks.Remarks>>(errMSg, remarksListing);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.Remarks.Remarks>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }


    }
}
