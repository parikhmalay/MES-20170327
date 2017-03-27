using Account.DTO.Library;
using MES.Business.Repositories.RFQ.Supplier;
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

namespace MES.Business.Library.BO.RFQ.Supplier
{
    class Contacts : ContextBusinessBase, IContactsRepository
    {
        public Contacts()
            : base("Contacts")
        {

        }

        public NPE.Core.ITypedResponse<int?> Save(DTO.Library.RFQ.Supplier.Contacts contacts)
        {
            string errMSg = null;
            string successMsg = null;

            var recordToBeUpdated = new MES.Data.Library.Contacts1();
            if (contacts.Id > 0)
            {
                recordToBeUpdated = this.DataContext.Contacts1.Where(a => a.Id == contacts.Id).SingleOrDefault();

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
                this.DataContext.Contacts1.Add(recordToBeUpdated);
            }
            if (string.IsNullOrEmpty(errMSg))
            {
                recordToBeUpdated.SupplierId = contacts.SupplierId;
                recordToBeUpdated.PrefixId = contacts.PrefixId.HasValue ? contacts.PrefixId.Value : (short?)null;
                recordToBeUpdated.FirstName = contacts.FirstName;
                recordToBeUpdated.LastName = contacts.LastName;
                recordToBeUpdated.Suffix = contacts.Suffix;
                recordToBeUpdated.Designation = contacts.Designation;
                recordToBeUpdated.DirectPhone = contacts.DirectPhone;
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

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>> GetRfqSupplierPartsQuote(string rfqId)
        {
            string errMSg = string.Empty;

            List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote> lstrFQSuppliersPartQuote = new List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>();
            DTO.Library.RFQ.RFQ.RFQSupplierPartQuote rfqSPQ = new DTO.Library.RFQ.RFQ.RFQSupplierPartQuote();
            this.RunOnDB(context =>
            {
                var rFQSPQList = context.GetPartsToQuote(rfqId).ToList();
                if (rFQSPQList == null)
                    errMSg = Languages.GetResourceText("RecordNotExist");
                else
                {
                    foreach (var item in rFQSPQList)
                    {


                        lstrFQSuppliersPartQuote.Add(rfqSPQ);
                    }
                }
            });

            //get hold of response
            var response = SuccessOrFailedResponse<List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuote>>(errMSg, lstrFQSuppliersPartQuote);
            return response;
        }


        public NPE.Core.ITypedResponse<DTO.Library.RFQ.Supplier.Contacts> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public NPE.Core.ITypedResponse<bool?> Delete(int contactsId)
        {
            var ContactsToBeDeleted = this.DataContext.Contacts1.Where(a => a.Id == contactsId).SingleOrDefault();
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
                context.DeleteMultipleContact(ContactIds, CurrentUser, Result);
            });
            if (Convert.ToInt32(Result.Value) > 0)
                return SuccessBoolResponse(Languages.GetResourceText("ContactsDeletedSuccess"));
            else
                return FailedBoolResponse(Languages.GetResourceText("ContactDeleteFail"));
        }

        public NPE.Core.ITypedResponse<List<DTO.Library.RFQ.Supplier.Contacts>> Search(NPE.Core.IPage<string> searchInfo)
        {
            throw new NotImplementedException();
        }
        public List<DTO.Library.RFQ.Supplier.Contacts> GetContactsList(int supplierId)
        {
            List<DTO.Library.RFQ.Supplier.Contacts> lstContact = new List<DTO.Library.RFQ.Supplier.Contacts>();
            DTO.Library.RFQ.Supplier.Contacts contacts;
            this.RunOnDB(context =>
            {
                var ContactsList = context.Contacts1.Where(c => c.SupplierId == supplierId && c.IsDeleted == false).OrderByDescending(a => a.CreatedDate).ToList();
                if (ContactsList != null)
                {
                    foreach (var item in ContactsList)
                    {
                        contacts = new DTO.Library.RFQ.Supplier.Contacts();
                        contacts.Id = item.Id;
                        contacts.SupplierId = item.SupplierId;
                        contacts.PrefixId = item.PrefixId;
                        contacts.FirstName = item.FirstName;
                        contacts.LastName = item.LastName;
                        contacts.Suffix = item.Suffix;
                        contacts.Designation = item.Designation;
                        contacts.DirectPhone = item.DirectPhone;
                        contacts.Extension = item.Extension;
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
