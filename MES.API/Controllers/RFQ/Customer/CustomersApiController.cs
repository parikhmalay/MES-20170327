using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.RFQ.Customer;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.RFQ.Customer
{
    [AdminPrefix("CustomersApi")]
    public class CustomersApiController : SecuredApiControllerBase
    {
        [Inject]
        public ICustomersRepository CustomersRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.Customer.Customers> Get(int Id)
        {
            var type = this.Resolve<ICustomersRepository>(CustomersRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Customer data.
        /// </summary>
        /// <param name="Customers"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RFQ.Customer.Customers customers)
        {
            var type = this.Resolve<ICustomersRepository>(CustomersRepository).Save(customers);
            return type;
        }

        /// <summary>
        /// delete the Customer data.
        /// </summary>
        /// <param name="CustomersId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int customersId)
        {
            var type = this.Resolve<ICustomersRepository>(CustomersRepository).Delete(customersId);
            return type;
        }

        /// <summary>
        /// Get Customers list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetCustomersList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.Customer.Customers>> GetCustomersList(GenericPage<MES.DTO.Library.RFQ.Customer.SearchCriteria> paging)
        {
            var type = this.Resolve<ICustomersRepository>(CustomersRepository).GetCustomersList(paging);
            return type;
        }
        /// <summary>
        /// delete multiple customer.
        /// </summary>
        /// <param name="CustomerIds"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteMultiple")]
        public ITypedResponse<bool?> DeleteMultiple(string CustomerIds)
        {
            var type = this.Resolve<ICustomersRepository>(CustomersRepository).DeleteMultiple(CustomerIds);
            return type;
        }
        #endregion Methods
    }
}