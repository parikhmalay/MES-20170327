using Account.DTO.Library;
using MES.Business.Repositories.Setup.RFQSource;
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


namespace MES.Business.Library.BO.Setup.RFQSource
{
    class RFQSource : ContextBusinessBase, IRFQSourceRepository
    {
        public RFQSource() : base("RFQSource") { }


        public ITypedResponse<bool?> Delete(int id)
        {
            var rfqSourceToBeDeleted = this.DataContext.RFQSources.Where(a => a.Id == id).SingleOrDefault();

            if (rfqSourceToBeDeleted == null)
                return FailedBoolResponse(Languages.GetResourceText("RFQSourceNotExists"));
            else
            {
                rfqSourceToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                rfqSourceToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(rfqSourceToBeDeleted).State = EntityState.Modified;

                rfqSourceToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("RFQSourceDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.RFQSource.RFQSource> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.RFQSource.RFQSource rfqSource)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.RFQSources.AsNoTracking().Any(a => a.RFQSource1 == rfqSource.rfqSource && a.IsDeleted == false && a.Id != rfqSource.Id))
            {
                errMSg = Languages.GetResourceText("RFQSourceExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.RFQSource();

                if (rfqSource.HasId())
                {
                    recordToBeUpdated = this.DataContext.RFQSources.Where(a => a.Id == rfqSource.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("RFQSourceNotExists");
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
                    this.DataContext.RFQSources.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.RFQSource1 = rfqSource.rfqSource;
                    this.DataContext.SaveChanges();
                    rfqSource.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("RFQSourceSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, rfqSource.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.RFQSource.RFQSource>> GetRFQSources(IPage<DTO.Library.Setup.RFQSource.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.RFQSource.RFQSource> rfqSources = new List<DTO.Library.Setup.RFQSource.RFQSource>();
            DTO.Library.Setup.RFQSource.RFQSource rfqSource;
            this.RunOnDB(context =>
            {
                var rfqSourceList = context.SearchRFQSources(paging.Criteria.rfqSource, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (rfqSourceList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in rfqSourceList)
                    {
                        rfqSource = new DTO.Library.Setup.RFQSource.RFQSource();
                        rfqSource.Id = item.Id;
                        rfqSource.rfqSource = item.RFQSource;
                        rfqSources.Add(rfqSource);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.RFQSource.RFQSource>>(errMSg, rfqSources);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.RFQSource.RFQSource>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
