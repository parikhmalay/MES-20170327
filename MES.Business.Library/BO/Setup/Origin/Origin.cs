using Account.DTO.Library;
using MES.Business.Repositories.Setup.Origin;
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

namespace MES.Business.Library.BO.Setup.Origin
{
    class Origin : ContextBusinessBase, IOriginRepository
    {
        public Origin() : base("Origin") { }

        public ITypedResponse<bool?> Delete(int id)
        {
            var originToBeDeleted = this.DataContext.Origins.Where(a => a.Id == id).SingleOrDefault();
            if (originToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("OriginNotExists"));
            }
            else
            {
                originToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                originToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(originToBeDeleted).State = EntityState.Modified;
                originToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("OriginDeletedSuccess"));
            }
        }

        public ITypedResponse<DTO.Library.Setup.Origin.Origin> FindById(int id)
        {
            string errMSg = string.Empty;

            DTO.Library.Setup.Origin.Origin origin = new DTO.Library.Setup.Origin.Origin();

            var originItem = this.DataContext.Origins.Where(a => a.Id == id).SingleOrDefault();
            if (originItem == null)
                errMSg = Languages.GetResourceText("OriginNotExists");
            else
            {
                #region general details
                origin = ObjectLibExtensions.AutoConvert<DTO.Library.Setup.Origin.Origin>(originItem);
                #endregion
            }
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.Setup.Origin.Origin>(errMSg, origin);
            return response;
        }

        public ITypedResponse<int?> Save(DTO.Library.Setup.Origin.Origin origin)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Origins.AsNoTracking().Any(a => a.Origin1 == origin.origin && a.IsDeleted == false && a.Id != origin.Id))
            {
                errMSg = Languages.GetResourceText("OriginExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Origin();

                if (origin.HasId())
                {
                    recordToBeUpdated = this.DataContext.Origins.Where(a => a.Id == origin.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("OriginNotExists");
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
                    this.DataContext.Origins.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.Origin1 = origin.origin;
                    this.DataContext.SaveChanges();
                    origin.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("OrigineSavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, origin.Id, successMsg);
        }

        public ITypedResponse<List<DTO.Library.Setup.Origin.Origin>> GetOrigins(IPage<DTO.Library.Setup.Origin.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);

            string errMSg = null;

            //declare paging variables
            /*int PageNumber = paging.PageNo > 0 ? paging.PageNo - 1 : 0;
            int PageSize = paging.PageSize > 0 ? paging.PageSize : 10;
            int RecordStart = PageNumber * PageSize;*/

            List<DTO.Library.Setup.Origin.Origin> origins = new List<DTO.Library.Setup.Origin.Origin>();
            DTO.Library.Setup.Origin.Origin origin;
            this.RunOnDB(context =>
            {
                var originList = context.SearchOrigins(paging.Criteria.origin, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                if (originList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    //setup total records
                    paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                    foreach (var item in originList)
                    {
                        origin = new DTO.Library.Setup.Origin.Origin();
                        origin.Id = item.Id;
                        origin.origin = item.Origin;
                        origins.Add(origin);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Origin.Origin>>(errMSg, origins);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }


        public ITypedResponse<List<DTO.Library.Setup.Origin.Origin>> Search(IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

    }
}
