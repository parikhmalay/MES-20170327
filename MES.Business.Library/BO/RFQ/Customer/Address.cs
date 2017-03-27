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
    class Address : ContextBusinessBase, IAddressRepository
    {
        public Address()
            : base("Address")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Customer.Address address)
        {
            string errMSg = null;
            string successMsg = null;

            var recordToBeUpdated = new MES.Data.Library.Address();
            if (address.Id > 0)
            {
                recordToBeUpdated = this.DataContext.Addresses.Where(a => a.Id == address.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("AddressNotExists");
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
                this.DataContext.Addresses.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.CustomerId = address.CustomerId;
                recordToBeUpdated.AddressTypeId = address.AddressTypeId;
                recordToBeUpdated.Address1 = address.Address1;
                recordToBeUpdated.Address2 = address.Address2;
                recordToBeUpdated.City = address.City;
                recordToBeUpdated.State = address.State;
                recordToBeUpdated.CountryId = address.CountryId;
                recordToBeUpdated.Zip = address.Zip;
                recordToBeUpdated.IsDefault = address.IsDefault;
                this.DataContext.SaveChanges();
                address.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("AddressSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, address.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Customer.Address> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.Customer.Address address = new DTO.Library.RFQ.Customer.Address();
            this.RunOnDB(context =>
            {
                var contactItem = context.Addresses.Where(r => r.Id == id && r.IsDeleted == false).SingleOrDefault();
                if (contactItem == null)
                    errMSg = Languages.GetResourceText("AddressNotExists");
                else
                {
                    #region general details
                    address.Id = contactItem.Id;
                    address.CustomerId = contactItem.CustomerId;
                    address.AddressTypeId = contactItem.AddressTypeId;
                    address.Address1 = contactItem.Address1;
                    address.Address2 = contactItem.Address2;
                    address.City = contactItem.City;
                    address.State= contactItem.State;
                    address.CountryId = contactItem.CountryId;
                    address.Zip = contactItem.Zip;
                    address.IsDefault = contactItem.IsDefault;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Customer.Address>(errMSg, address);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int addressId)
        {
            var AddressToBeDeleted = this.DataContext.Addresses.Where(a => a.Id == addressId).SingleOrDefault();
            if (AddressToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("AddressNotExists"));
            }
            else
            {
                AddressToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                AddressToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(AddressToBeDeleted).State = EntityState.Modified;
                AddressToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("AddressDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<bool?> DeleteMultipleAddress(string addressIds)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                context.DeleteMultipleCustomerAddress(addressIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("AddressDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("AddressDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Customer.Address>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.RFQ.Customer.Address> GetAddressList(int customerId)
        {
            List<DTO.Library.RFQ.Customer.Address> lstAddress = new List<DTO.Library.RFQ.Customer.Address>();
            DTO.Library.RFQ.Customer.Address address;
            this.RunOnDB(context =>
            {
                var AddressList = context.Addresses.Where(c => c.CustomerId == customerId && c.IsDeleted == false).OrderByDescending(a => a.CreatedDate).ToList();
                if (AddressList != null)
                {
                    foreach (var item in AddressList)
                    {
                        address = new DTO.Library.RFQ.Customer.Address();
                        address.Id = item.Id;
                        address.CustomerId = item.CustomerId;
                        address.AddressTypeId = item.AddressTypeId;
                        address.Address1 = item.Address1;
                        address.Address2 = item.Address2;
                        address.City = item.City;
                        address.State = item.State;
                        address.CountryId = item.CountryId;
                        address.Zip = item.Zip;
                        address.IsDefault = item.IsDefault;
                        address.AddressTypeItem = new DTO.Library.Common.LookupFields
                        {
                            Id = item.AddressTypeId,
                            Name = item.AddressType.Name
                        };
                        lstAddress.Add(address);
                    }
                }
            });
            return lstAddress;
        }
    }
}
