using Account.DTO.Library;
using MES.Business.Repositories.Setup.CommodityType;
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

namespace MES.Business.Library.BO.Setup.CommodityType
{
    class CommodityType : ContextBusinessBase, ICommodityTypeRepository
    {
        public CommodityType()
            : base("CommodityType")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.Setup.CommodityType.CommodityType commodityType)
        {
            string errMSg = null;
            string successMsg = null;
            try
            {
                //check for the uniqueness
                if (this.DataContext.CommodityTypes.AsNoTracking().Any(a => a.CommodityType1 == commodityType.commodityType && a.IsDeleted == false && a.Id != commodityType.Id))
                {
                    errMSg = Languages.GetResourceText("CommodityTypeExists");
                }
                else
                {
                    var recordToBeUpdated = new MES.Data.Library.CommodityType();
                    if (commodityType.Id > 0)
                    {
                        recordToBeUpdated = this.DataContext.CommodityTypes.Where(a => a.Id == commodityType.Id).SingleOrDefault();
                        if (recordToBeUpdated == null)
                            errMSg = Languages.GetResourceText("CommodityTypeNotExists");
                        else
                        {
                            #region "Delete commodity type customers Details"
                            var deleteCommodityTypeCustomerList = this.DataContext.CommodityTypeCustomers.Where(a => a.CommodityTypeId == commodityType.Id).ToList();
                            foreach (var item in deleteCommodityTypeCustomerList)
                            {
                                this.DataContext.CommodityTypeCustomers.Remove(item);
                            }
                            #endregion
                            #region "Delete commodity type suppliers Details"
                            var deleteCommodityTypeSupplierList = this.DataContext.CommodityTypeSuppliers.Where(a => a.CommodityTypeId == commodityType.Id).ToList();
                            foreach (var item in deleteCommodityTypeSupplierList)
                            {
                                this.DataContext.CommodityTypeSuppliers.Remove(item);
                            }
                            #endregion

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
                        this.DataContext.CommodityTypes.Add(recordToBeUpdated);
                    }
                    if (string.IsNullOrEmpty(errMSg))
                    {
                        recordToBeUpdated.CommodityType1 = commodityType.commodityType;
                        this.DataContext.SaveChanges();
                        commodityType.Id = recordToBeUpdated.Id;

                        #region "Save commodity type customers Details"
                        MES.Data.Library.CommodityTypeCustomer dboCommodityTypeCustomer = null;
                        if (commodityType.CommodityTypeCustomerList != null && commodityType.CommodityTypeCustomerList.Count > 0)
                        {
                            bool AnyCommodityTypeCustomer = false;
                            foreach (var commodityTypeCustomer in commodityType.CommodityTypeCustomerList)
                            {
                                if (commodityTypeCustomer.Id != 0)
                                {
                                    AnyCommodityTypeCustomer = true;
                                    dboCommodityTypeCustomer = new MES.Data.Library.CommodityTypeCustomer();
                                    dboCommodityTypeCustomer.CommodityTypeId = (short)commodityType.Id;
                                    dboCommodityTypeCustomer.CustomerId = commodityTypeCustomer.Id;
                                    dboCommodityTypeCustomer.CreatedBy = CurrentUser;
                                    dboCommodityTypeCustomer.CreatedDate = AuditUtils.GetCurrentDateTime();
                                    this.DataContext.CommodityTypeCustomers.Add(dboCommodityTypeCustomer);
                                }
                            }
                            if (AnyCommodityTypeCustomer)
                                this.DataContext.SaveChanges();
                        }
                        #endregion
                        #region "Save commodity type supplier Details"
                        MES.Data.Library.CommodityTypeSupplier dboCommodityTypeSupplier = null;
                        if (commodityType.CommodityTypeSupplierList != null && commodityType.CommodityTypeSupplierList.Count > 0)
                        {
                            bool AnyCommodityTypeSupplier = false;
                            foreach (var commodityTypeSupplier in commodityType.CommodityTypeSupplierList)
                            {
                                if (commodityTypeSupplier.Id != 0)
                                {
                                    AnyCommodityTypeSupplier = true;
                                    dboCommodityTypeSupplier = new MES.Data.Library.CommodityTypeSupplier();
                                    dboCommodityTypeSupplier.CommodityTypeId = (short)commodityType.Id;
                                    dboCommodityTypeSupplier.SupplierId = commodityTypeSupplier.Id;
                                    dboCommodityTypeSupplier.CreatedBy = CurrentUser;
                                    dboCommodityTypeSupplier.CreatedDate = AuditUtils.GetCurrentDateTime();
                                    this.DataContext.CommodityTypeSuppliers.Add(dboCommodityTypeSupplier);
                                }
                            }
                            if (AnyCommodityTypeSupplier)
                                this.DataContext.SaveChanges();
                        }
                        #endregion

                        successMsg = Languages.GetResourceText("CommodityTypeSavedSuccess");
                    }
                }
                return SuccessOrFailedResponse<int?>(errMSg, commodityType.Id, successMsg);
            }
            catch (Exception ex)
            {
                errMSg = ex.Message.ToString();
                return SuccessOrFailedResponse<int?>(errMSg, commodityType.Id, successMsg);
            }
        }

        public NPE.Core.ITypedResponse<DTO.Library.Setup.CommodityType.CommodityType> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int commodityTypeId)
        {
            var commodityTypeToBeDeleted = this.DataContext.CommodityTypes.Where(a => a.Id == commodityTypeId).SingleOrDefault();
            if (commodityTypeToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("CommodityTypeNotExists"));
            }
            else
            {
                commodityTypeToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                commodityTypeToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(commodityTypeToBeDeleted).State = EntityState.Modified;
                commodityTypeToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("CommodityTypeDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.CommodityType.CommodityType>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.Setup.CommodityType.CommodityType>> GetCommodityTypesList(NPE.Core.IPage<DTO.Library.Setup.CommodityType.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.Setup.CommodityType.CommodityType> lstcommodityType = new List<DTO.Library.Setup.CommodityType.CommodityType>();
            DTO.Library.Setup.CommodityType.CommodityType commodityType;
            this.RunOnDB(context =>
             {
                 var commodityTypeList = context.SearchCommodityType(paging.Criteria.commodityType, paging.Criteria.SupplierId, paging.Criteria.CustomerId, paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (commodityTypeList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in commodityTypeList)
                     {
                         commodityType = new DTO.Library.Setup.CommodityType.CommodityType();
                         commodityType.Id = item.Id;
                         commodityType.commodityType = item.CommodityType;
                         #region Bind Customer details
                         commodityType.CommodityTypeCustomerList = new List<DTO.Library.Setup.CommodityType.CommodityTypeCustomer>();
                         context.CommodityTypeCustomers.Where(a => a.CommodityTypeId == item.Id).ToList().ForEach(tl => commodityType.CommodityTypeCustomerList.Add(
                             new DTO.Library.Setup.CommodityType.CommodityTypeCustomer()
                             {
                                 Id = Convert.ToInt32(tl.CustomerId),
                                 Name = tl.Customer.CompanyName,
                                 CommodityTypeId = Convert.ToInt32(tl.CommodityTypeId),
                             })); 
                         #endregion
                         #region Bind Supplier details
                         commodityType.CommodityTypeSupplierList = new List<DTO.Library.Setup.CommodityType.CommodityTypeSupplier>();
                         context.CommodityTypeSuppliers.Where(a => a.CommodityTypeId == item.Id).ToList().ForEach(tl => commodityType.CommodityTypeSupplierList.Add(
                             new DTO.Library.Setup.CommodityType.CommodityTypeSupplier()
                             {
                                 Id = Convert.ToInt32(tl.SupplierId),
                                 Name = tl.Supplier.CompanyName,
                                 CommodityTypeId = Convert.ToInt32(tl.CommodityTypeId),
                             }));
                         #endregion
                         lstcommodityType.Add(commodityType);
                     }
                 }
             });

            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.Setup.CommodityType.CommodityType>>(errMSg, lstcommodityType);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
