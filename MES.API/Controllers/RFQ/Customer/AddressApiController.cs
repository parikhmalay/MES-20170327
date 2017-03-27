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
    [AdminPrefix("CustomerAddressApi")]
    public class CustomerAddressApiController : SecuredApiControllerBase
    {
        [Inject]
        public IAddressRepository AddressRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.Customer.Address> Get(int Id)
        {
            var type = this.Resolve<IAddressRepository>(AddressRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Address data.
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RFQ.Customer.Address address)
        {
            var type = this.Resolve<IAddressRepository>(AddressRepository).Save(address);
            return type;
        }

        /// <summary>
        /// delete Address.
        /// </summary>
        /// <param name="AddressId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int addressId)
        {
            var type = this.Resolve<IAddressRepository>(AddressRepository).Delete(addressId);
            return type;
        }
        /// <summary>
        /// delete multiple Address.
        /// </summary>
        /// <param name="AddressId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteMultipleAddress")]
        public ITypedResponse<bool?> DeleteMultipleAddress(string addressIds)
        {
            var type = this.Resolve<IAddressRepository>(AddressRepository).DeleteMultipleAddress(addressIds);
            return type;
        }
        /// <summary>
        /// Get Address list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetAddressList")]
        public List<MES.DTO.Library.RFQ.Customer.Address> GetAddressList(int customerId)
        {
            var type = this.Resolve<IAddressRepository>(AddressRepository).GetAddressList(customerId);
            return type;
        }
        #endregion Methods
    }
}