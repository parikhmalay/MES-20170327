using Account.DTO.Library;
using MES.Business.Repositories.Setup.Commodity;
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

namespace MES.Business.Library.BO.Setup.Commodity
{
    class Commodity : ContextBusinessBase, ICommodityRepository
    {
        public Commodity()
            : base("Commodity")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.Commodity.Commodity commodity)
        {
            string errMSg = null;
            string successMsg = null;
            //check for the uniqueness
            if (this.DataContext.Commodities.AsNoTracking().Any(a => a.CommodityName == commodity.CommodityName && a.IsDeleted == false && a.Id != commodity.Id))
            {
                errMSg = Languages.GetResourceText("CommodityExists");
            }
            else
            {
                var recordToBeUpdated = new MES.Data.Library.Commodity();

                if (commodity.Id > 0)
                {
                    recordToBeUpdated = this.DataContext.Commodities.Where(a => a.Id == commodity.Id).SingleOrDefault();

                    if (recordToBeUpdated == null)
                        errMSg = Languages.GetResourceText("CommodityNotExists");
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
                    this.DataContext.Commodities.Add(recordToBeUpdated);
                }
                if (string.IsNullOrEmpty(errMSg))
                {
                    recordToBeUpdated.CommodityName = commodity.CommodityName;
                    recordToBeUpdated.CategoryId = commodity.CategoryId;
                    this.DataContext.SaveChanges();
                    commodity.Id = recordToBeUpdated.Id;
                    successMsg = Languages.GetResourceText("CommoditySavedSuccess");
                }
            }
            return SuccessOrFailedResponse<int?>(errMSg, commodity.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.Commodity.Commodity> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int commodityId)
        {
            var commodityToBeDeleted = this.DataContext.Commodities.Where(a => a.Id == commodityId).SingleOrDefault();
            if (commodityToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("CommodityNotExists"));
            }
            else
            {
                commodityToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                commodityToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(commodityToBeDeleted).State = EntityState.Modified;
                commodityToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("CommodityDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Commodity.Commodity>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.Commodity.Commodity>> GetCommodityList(NPE.Core.IPage<DTO.Library.Setup.Commodity.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.Commodity.Commodity> lstcommodity = new List<DTO.Library.Setup.Commodity.Commodity>();
            DTO.Library.Setup.Commodity.Commodity commodity;
            this.RunOnDB(context =>
             {
                 var commodityList = context.SearchCommodity(paging.Criteria.CommodityName, paging.Criteria.CategoryId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (commodityList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in commodityList)
                     {
                         commodity = new DTO.Library.Setup.Commodity.Commodity();
                         commodity.Id = item.Id;
                         commodity.CommodityName = item.CommodityName;
                         commodity.CategoryId = item.CategoryId;
                         commodity.CategoryName = item.CategoryName;
                         lstcommodity.Add(commodity);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.Commodity.Commodity>>(errMSg, lstcommodity);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
