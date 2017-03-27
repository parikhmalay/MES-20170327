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
    [AdminPrefix("SuppliersApi")]
    public class SuppliersApiController : SecuredApiControllerBase
    {
        [Inject]
        public ISuppliersRepository SuppliersRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.Supplier.Suppliers> Get(int Id)
        {
            var type = this.Resolve<ISuppliersRepository>(SuppliersRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Supplier data.
        /// </summary>
        /// <param name="Suppliers"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RFQ.Supplier.Suppliers suppliers)
        {
            var type = this.Resolve<ISuppliersRepository>(SuppliersRepository).Save(suppliers);
            return type;
        }

        /// <summary>
        /// delete the Supplier data.
        /// </summary>
        /// <param name="SuppliersId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int suppliersId)
        {
            var type = this.Resolve<ISuppliersRepository>(SuppliersRepository).Delete(suppliersId);
            return type;
        }

        /// <summary>
        /// Get Suppliers list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetSuppliersList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.Supplier.Suppliers>> GetSuppliersList(GenericPage<MES.DTO.Library.RFQ.Supplier.SearchCriteria> paging)
        {
            var type = this.Resolve<ISuppliersRepository>(SuppliersRepository).GetSuppliersList(paging);
            return type;
        }

        /// <summary>
        /// send email.
        /// </summary>
        /// <param name="emailIdsList"></param>
        /// <returns></returns>
        [HttpPostRoute("SendEmail")]
        public ITypedResponse<bool?> SendEmail(MES.DTO.Library.Common.EmailData EmailData)
        {
            var type = this.Resolve<ISuppliersRepository>(SuppliersRepository).SendEmail(EmailData);
            return type;
        }
        /// <summary>
        /// delete multiple supplier.
        /// </summary>
        /// <param name="SupplierIds"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteMultiple")]
        public ITypedResponse<bool?> DeleteMultiple(string SupplierIds)
        {
            var type = this.Resolve<ISuppliersRepository>(SuppliersRepository).DeleteMultiple(SupplierIds);
            return type;
        }
        #endregion Methods
    }
}