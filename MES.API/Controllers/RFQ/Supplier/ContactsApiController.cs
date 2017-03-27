using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.RFQ.Supplier;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.RFQ.Supplier
{
    [AdminPrefix("ContactsApi")]
    public class ContactsApiController : SecuredApiControllerBase
    {
        [Inject]
        public IContactsRepository ContactsRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.Supplier.Contacts> Get(int Id)
        {
            var type = this.Resolve<IContactsRepository>(ContactsRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the contact data.
        /// </summary>
        /// <param name="Contacts"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RFQ.Supplier.Contacts contacts)
        {
            var type = this.Resolve<IContactsRepository>(ContactsRepository).Save(contacts);
            return type;
        }

        /// <summary>
        /// delete contact.
        /// </summary>
        /// <param name="ContactsId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int contactId)
        {
            var type = this.Resolve<IContactsRepository>(ContactsRepository).Delete(contactId);
            return type;
        }
        /// <summary>
        /// delete multiple contact.
        /// </summary>
        /// <param name="ContactsId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteMultiple")]
        public ITypedResponse<bool?> DeleteMultiple(string ContactIds)
        {
            var type = this.Resolve<IContactsRepository>(ContactsRepository).DeleteMultiple(ContactIds);
            return type;
        }
        /// <summary>
        /// Get Contacts list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetContactsList")]
        public List<MES.DTO.Library.RFQ.Supplier.Contacts> GetContactsList(int supplierId)
        {
            var type = this.Resolve<IContactsRepository>(ContactsRepository).GetContactsList(supplierId);
            return type;
        }
        #endregion Methods
    }
}