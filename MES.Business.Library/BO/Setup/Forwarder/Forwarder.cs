using Account.DTO.Library;
using MES.Business.Repositories.Setup.Forwarder;
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

namespace MES.Business.Library.BO.Setup.Forwarder
{
    class Forwarder : ContextBusinessBase, IForwarderRepository
    {
        public Forwarder()
            : base("Forwarder")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.Forwarder.Forwarder forwarder)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Forwarders.AsNoTracking().Any(a => a.ForwarderName == forwarder.ForwarderName && a.IsDeleted == false && a.Id != forwarder.Id))
            {
                errMSg = Languages.GetResourceText("ForwarderExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Forwarder();

                if (forwarder.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.Forwarders.Where(a => a.Id == forwarder.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("ForwarderNotExists");
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
                    this.DataContext.Forwarders.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.ForwarderName = forwarder.ForwarderName;
                    this.DataContext.SaveChanges();
                    forwarder.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("ForwarderSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, forwarder.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.Forwarder.Forwarder> FindById(int id)
        {
            string errMSg = string.Empty;

            DTO.Library.Setup.Forwarder.Forwarder origin = new DTO.Library.Setup.Forwarder.Forwarder();

            var originItem = this.DataContext.Forwarders.Where(a => a.Id == id).SingleOrDefault();
            if (originItem == null)
                errMSg = Languages.GetResourceText("ForwarderNotExists");
            else
            {
                #region general details
                origin = ObjectLibExtensions.AutoConvert<DTO.Library.Setup.Forwarder.Forwarder>(originItem);
                #endregion
            }
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.Forwarder.Forwarder>(errMSg, origin);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int forwarderId)
        {
            var ForwarderToBeDeleted = this.DataContext.Forwarders.Where(a => a.Id == forwarderId).SingleOrDefault();
            if (ForwarderToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("ForwarderNotExists"));
            }
            else
            {
                ForwarderToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                ForwarderToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(ForwarderToBeDeleted).State = EntityState.Modified;
                ForwarderToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("ForwarderDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Forwarder.Forwarder>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Forwarder.Forwarder>> GetForwarderList(NPE.Core.IPage<DTO.Library.Setup.Forwarder.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.Forwarder.Forwarder> lstForwarder = new List<DTO.Library.Setup.Forwarder.Forwarder>();
            DTO.Library.Setup.Forwarder.Forwarder forwarder;
            this.RunOnDB(context =>
             {
                 var ForwarderList = context.SearchForwarder(paging.Criteria.ForwarderName, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (ForwarderList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in ForwarderList)
                     {
                         forwarder = new DTO.Library.Setup.Forwarder.Forwarder();
                         forwarder.Id = item.Id;
                         forwarder.ForwarderName = item.ForwarderName;
                         lstForwarder.Add(forwarder);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Forwarder.Forwarder>>(errMSg, lstForwarder);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }

    }
}
