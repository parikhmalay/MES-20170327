using Account.DTO.Library;
using MES.Business.Repositories.RFQ.Customer;
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
using System.Data.Entity.Validation;

namespace MES.Business.Library.BO.RFQ.Customer
{
    class Customers : ContextBusinessBase, ICustomersRepository
    {
        public Customers()
            : base("Customers")
        { }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Customer.Customers customers)
        {
            string errMSg = null;
            string successMsg = null;
            var recordToBeUpdated = new MES.Data.Library.Customer();
            bool IsNewRecord = true;
            if (customers.Id > 0)
            {
                recordToBeUpdated = this.DataContext.Customers.Where(a => a.Id == customers.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("CustomersNotExists");
                else
                {
                    IsNewRecord = false;
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
                this.DataContext.Customers.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                #region Save general details
                recordToBeUpdated.CustomerCode = customers.CustomerCode;
                recordToBeUpdated.CompanyName = customers.CompanyName;
                recordToBeUpdated.Website = customers.Website;
                recordToBeUpdated.CompanyPhone1 = customers.CompanyPhone1;
                recordToBeUpdated.CompanyPhone2 = customers.CompanyPhone2;
                recordToBeUpdated.CompanyFax = customers.CompanyFax;
                recordToBeUpdated.Comments = customers.Comments;
                recordToBeUpdated.PaymentRating = customers.PaymentRating;
                recordToBeUpdated.SAMId = customers.SAMId;
                this.DataContext.SaveChanges();
                customers.Id = recordToBeUpdated.Id;
                #endregion

                #region "Save customer contacts Detail"
                if (IsNewRecord && customers.lstContact != null && customers.lstContact.Count > 0)
                {
                    MES.Business.Repositories.RFQ.Customer.IContactsRepository objIContactsRepository = null;
                    foreach (var objContact in customers.lstContact)
                    {
                        objIContactsRepository = new MES.Business.Library.BO.RFQ.Customer.Contacts();
                        objContact.CustomerId = customers.Id;
                        objIContactsRepository.Save(objContact);
                    }
                }
                #endregion

                #region "Save customer division Detail"
                if (IsNewRecord && customers.lstDivision != null && customers.lstDivision.Count > 0)
                {
                    MES.Business.Repositories.RFQ.Customer.IDivisionsRepository objIDivisionsRepository = null;
                    foreach (var objDivision in customers.lstDivision)
                    {
                        if (!string.IsNullOrEmpty(objDivision.CompanyName))
                        {
                            objIDivisionsRepository = new MES.Business.Library.BO.RFQ.Customer.Divisions();
                            objDivision.CustomerId = customers.Id;
                            objIDivisionsRepository.Save(objDivision);
                        }
                    }
                }
                #endregion

                #region "Save customer address Detail"
                if (IsNewRecord && customers.lstAddress != null && customers.lstAddress.Count > 0)
                {
                    MES.Business.Repositories.RFQ.Customer.IAddressRepository objIAddressRepository = null;
                    foreach (var objAddress in customers.lstAddress)
                    {
                        if (objAddress.AddressTypeId > 0)
                        {
                            objIAddressRepository = new MES.Business.Library.BO.RFQ.Customer.Address();
                            objAddress.CustomerId = customers.Id;
                            objIAddressRepository.Save(objAddress);
                        }
                    }
                }
                #endregion
                successMsg = Languages.GetResourceText("CustomersSavedSuccess");
            }
            return SuccessOrFailedResponse<int?>(errMSg, customers.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Customer.Customers> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.Customer.Customers customers = new DTO.Library.RFQ.Customer.Customers();
            this.RunOnDB(context =>
            {
                var Customers = context.Customers.Where(s => s.Id == id).SingleOrDefault();
                if (Customers == null)
                    errMSg = Languages.GetResourceText("CustomersNotExists");
                else
                {
                    #region general details
                    customers.Id = Customers.Id;
                    customers.CustomerCode = Customers.CustomerCode;
                    customers.CompanyName = Customers.CompanyName;
                    //customers.Address1 = Customers.Address1;
                    //customers.Address2 = Customers.Address2;
                    //customers.City = Customers.City;
                    //customers.State = Customers.State;
                    //customers.CountryId = Customers.CountryId;
                    //customers.Zip = Customers.Zip;
                    customers.Website = Customers.Website;
                    customers.CompanyPhone1 = Customers.CompanyPhone1;
                    customers.CompanyPhone2 = Customers.CompanyPhone2;
                    customers.CompanyFax = Customers.CompanyFax;
                    customers.Comments = Customers.Comments;
                    customers.PaymentRating = Customers.PaymentRating;
                    customers.SAMId = Customers.SAMId;
                    #endregion
                    #region Bind contact details
                    MES.Business.Repositories.RFQ.Customer.IContactsRepository objIContactsRepository = new MES.Business.Library.BO.RFQ.Customer.Contacts();
                    customers.lstContact = objIContactsRepository.GetContactsList(id);
                    #endregion
                    #region Bind division details
                    MES.Business.Repositories.RFQ.Customer.IDivisionsRepository objIDivisionsRepository = new MES.Business.Library.BO.RFQ.Customer.Divisions();
                    customers.lstDivision = objIDivisionsRepository.GetDivisionsList(id);
                    #endregion
                    #region Bind address details
                    MES.Business.Repositories.RFQ.Customer.IAddressRepository objIAddressRepository = new MES.Business.Library.BO.RFQ.Customer.Address();
                    customers.lstAddress = objIAddressRepository.GetAddressList(id);
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Customer.Customers>(errMSg, customers);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int customersId)
        {
            return DeleteMultiple(Convert.ToString(customersId));
        }

        public NPE.Core.ITypedResponse<bool?> DeleteMultiple(string CustomerIds)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                context.DeleteMultipleCustomer(CustomerIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("CustomersDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("CustomerDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Customer.Customers>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Customer.Customers>> GetCustomersList(NPE.Core.IPage<DTO.Library.RFQ.Customer.SearchCriteria> paging)
        {
            //set the out put param
            ObjectParameter totalRecords = new ObjectParameter("TotalRecords", 0);
            string errMSg = null;
            List<DTO.Library.RFQ.Customer.Customers> lstCustomers = new List<DTO.Library.RFQ.Customer.Customers>();
            DTO.Library.RFQ.Customer.Customers customers;
            this.RunOnDB(context =>
             {
                 var CustomersList = context.SearchCustomers(
                     paging.Criteria.CompanyName,
                     paging.Criteria.City,
                     paging.Criteria.State,
                     paging.Criteria.Website,
                     paging.Criteria.CompanyPhone1,
                    paging.Criteria.PaymentRating,
                     paging.PageNo, paging.PageSize, totalRecords, "").ToList();
                 if (CustomersList == null)
                     errMSg = Languages.GetResourceText("RecordNotExist");
                 else
                 {
                     //setup total records
                     paging.TotalRecords = Convert.ToInt32(totalRecords.Value);
                     foreach (var item in CustomersList)
                     {
                         customers = new DTO.Library.RFQ.Customer.Customers();
                         customers.Id = item.Id;
                         customers.CompanyName = item.CompanyName;
                         customers.City = item.City;
                         customers.State = item.State;
                         customers.Email = item.Email;
                         customers.Website = item.Website;
                         customers.CompanyPhone1 = item.CompanyPhone1;
                         lstCustomers.Add(customers);
                     }
                 }
             });
            //get hold of response
            var response = SuccessOrFailedResponse<List<MES.DTO.Library.RFQ.Customer.Customers>>(errMSg, lstCustomers);
            //populate page property
            response.PageInfo = paging.ToPage();
            return response;
        }
    }
}
