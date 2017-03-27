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
    class Contacts : ContextBusinessBase, IContactsRepository
    {
        public Contacts()
            : base("Contacts")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Customer.Contacts contacts)
        {
            string errMSg = null;
            string successMsg = null;

            var recordToBeUpdated = new MES.Data.Library.Contact();
            if (contacts.Id > 0)
            {
                recordToBeUpdated = this.DataContext.Contacts.Where(a => a.Id == contacts.Id).SingleOrDefault();

                if (recordToBeUpdated == null)
                    errMSg = Languages.GetResourceText("ContactsNotExists");
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
                this.DataContext.Contacts.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.CustomerId = contacts.CustomerId;
                recordToBeUpdated.PrefixId = contacts.PrefixId;
                recordToBeUpdated.FirstName = contacts.FirstName;
                recordToBeUpdated.LastName = contacts.LastName;
                recordToBeUpdated.Suffix = contacts.Suffix;
                recordToBeUpdated.Designation = contacts.Designation;
                recordToBeUpdated.DirectPhone = contacts.DirectPhone;
                recordToBeUpdated.CellPhone = contacts.CellPhone;
                recordToBeUpdated.Extension = contacts.Extension;
                recordToBeUpdated.DirectFax = contacts.DirectFax;
                recordToBeUpdated.Email = contacts.Email;
                recordToBeUpdated.Comments = contacts.Comments;
                recordToBeUpdated.IsDefault = contacts.IsDefault;
                this.DataContext.SaveChanges();
                contacts.Id = recordToBeUpdated.Id;
                successMsg = Languages.GetResourceText("ContactsSavedSuccess");
            }

            return SuccessOrFailedResponse<int?>(errMSg, contacts.Id, successMsg);
        }

        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Customer.Contacts> FindById(int id)
        {
            string errMSg = string.Empty;
            DTO.Library.RFQ.Customer.Contacts contact = new DTO.Library.RFQ.Customer.Contacts();
            this.RunOnDB(context =>
            {
                var contactItem = context.Contacts.Where(r => r.Id == id && r.IsDeleted == false).SingleOrDefault();
                if (contactItem == null)
                    errMSg = Languages.GetResourceText("CustomerContactNotExists");
                else
                {
                    #region general details
                    contact.Id = contactItem.Id;
                    contact.FirstName = contactItem.FirstName;
                    contact.LastName = contactItem.LastName;
                    contact.Email = contactItem.Email;
                    contact.DirectPhone = contactItem.DirectPhone;
                    #endregion
                }
            });
            //get hold of response
            var response = SuccessOrFailedResponse<MES.DTO.Library.RFQ.Customer.Contacts>(errMSg, contact);
            return response;
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int contactsId)
        {
            var ContactsToBeDeleted = this.DataContext.Contacts.Where(a => a.Id == contactsId).SingleOrDefault();
            if (ContactsToBeDeleted == null)
            {
                return FailedBoolResponse(Languages.GetResourceText("ContactsNotExists"));
            }
            else
            {
                ContactsToBeDeleted.UpdatedDate = AuditUtils.GetCurrentDateTime();
                ContactsToBeDeleted.UpdatedBy = CurrentUser;
                this.DataContext.Entry(ContactsToBeDeleted).State = EntityState.Modified;
                ContactsToBeDeleted.IsDeleted = true;
                this.DataContext.SaveChanges();
                return SuccessBoolResponse(Languages.GetResourceText("ContactsDeletedSuccess"));
            }
        }

        public NPE.Core.ITypedResponse<bool?> DeleteMultiple(string ContactIds)
        {
            //set the out put param
            ObjectParameter Result = new ObjectParameter("Result", 0);
            this.RunOnDB(context =>
            {
                context.DeleteMultipleCustomerContact(ContactIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("ContactsDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("ContactDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Customer.Contacts>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Library.RFQ.Customer.Contacts> GetContactsList(int customerId)
        {
            List<DTO.Library.RFQ.Customer.Contacts> lstContact = new List<DTO.Library.RFQ.Customer.Contacts>();
            DTO.Library.RFQ.Customer.Contacts contacts;
            this.RunOnDB(context =>
            {
                var ContactsList = context.Contacts.Where(c => c.CustomerId == customerId && c.IsDeleted == false).OrderByDescending(a => a.CreatedDate).ToList();
                if (ContactsList != null)
                {
                    foreach (var item in ContactsList)
                    {
                        contacts = new DTO.Library.RFQ.Customer.Contacts();
                        contacts.Id = item.Id;
                        contacts.CustomerId = item.CustomerId;
                        contacts.PrefixId = item.PrefixId;
                        contacts.FirstName = item.FirstName;
                        contacts.LastName = item.LastName;
                        contacts.Suffix = item.Suffix;
                        contacts.Designation = item.Designation;
                        contacts.DirectPhone = item.DirectPhone;
                        contacts.Extension = item.Extension;
                        contacts.CellPhone = item.CellPhone;
                        contacts.DirectFax = item.DirectFax;
                        contacts.Email = item.Email;
                        contacts.Comments = item.Comments;
                        contacts.IsDefault = item.IsDefault;

                        lstContact.Add(contacts);
                    }
                }
            });
            return lstContact;
        }
    }
}
