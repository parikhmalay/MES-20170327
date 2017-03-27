using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.RFQ.RFQ;
using NPE.Core;
using NPE.Core.Extended;
using MES.Business.Repositories.RFQ.Quote;
namespace MES.API.Controllers.RFQ.RFQ
{
    [AdminPrefix("RFQApi")]
    public class RFQApiController : SecuredApiControllerBase
    {
        [Inject]
        public IRFQRepository RFQRepository { get; set; }
        public IRFQPartsRepository RFQPartsRepository { get; set; }
        public IRFQSuppliersRepository RFQSuppliersRepository { get; set; }
        public IQuoteRepository QuoteRepository { get; set; }
        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQ> Get(string Id, string isR)
        {
            ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQ> type = null;
            if (isR == "0")
                type = this.Resolve<IRFQRepository>(RFQRepository).FindById(Id);
            else
                type = this.Resolve<IRFQRepository>(RFQRepository).CreateRevision(Id);
            return type;
        }
        /// <summary>
        /// Get QuoteToCustomer List.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("GetQuoteToCustomerList")]
        public ITypedResponse<List<DTO.Library.RFQ.Quote.Quote>> GetQuoteToCustomerList(string rfqId)
        {
            ITypedResponse<List<DTO.Library.RFQ.Quote.Quote>> type = null;
            type = this.Resolve<IQuoteRepository>(QuoteRepository).GetQuotesToCustomer(rfqId);
            return type;
        }

        /// <summary>
        /// save the RFQ data.
        /// </summary>
        /// <param name="RFQ"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<string> Save(MES.DTO.Library.RFQ.RFQ.RFQ RFQ)
        {
            var type = this.Resolve<IRFQRepository>(RFQRepository).Save(RFQ);
            return type;
        }
        /// <summary>
        /// save the RFQ data.
        /// </summary>
        /// <param name="RFQ"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveRfqPart")]
        public ITypedResponse<int?> SaveRfqPart(MES.DTO.Library.RFQ.RFQ.RFQParts rfqPart)
        {
            var type = this.Resolve<IRFQPartsRepository>(RFQPartsRepository).Save(rfqPart);
            return type;
        }
        /// <summary>
        /// delete the RFQ data.
        /// </summary>
        /// <param name="RFQId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(string RFQId)
        {
            var type = this.Resolve<IRFQRepository>(RFQRepository).Delete(RFQId);
            return type;
        }
        /// <summary>
        /// delete the RFQ data.
        /// </summary>
        /// <param name="RFQId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteRfqPart")]
        public ITypedResponse<bool?> DeleteRfqPart(int rfqPartId)
        {
            var type = this.Resolve<IRFQPartsRepository>(RFQPartsRepository).Delete(rfqPartId);
            return type;
        }
        /// <summary>
        /// upload RFQ Parts.
        /// </summary>
        /// <param name="RFQ"></param>
        /// <returns></returns>
        [HttpPostRoute("UploadRFQParts")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQParts>> UploadRFQParts(MES.DTO.Library.RFQ.RFQ.RFQ RFQ)
        {
            var type = this.Resolve<IRFQPartsRepository>(RFQPartsRepository).UploadRFQParts(RFQ);
            return type;
        }
        /// <summary>
        /// Get RFQ list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRFQList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQ>> GetRFQList(GenericPage<MES.DTO.Library.RFQ.RFQ.SearchCriteria> paging)
        {
            var type = this.Resolve<IRFQRepository>(RFQRepository).GetRFQList(paging);
            return type;
        }
        [HttpPostRoute("GetQuotedSuppliers")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>> GetQuotedSuppliers(string rfqId)
        {
            var type = this.Resolve<IRFQSuppliersRepository>(RFQSuppliersRepository).GetQuotedSuppliers(rfqId);
            return type;
        }
        [HttpPostRoute("GetQuotesToCustomer")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.Quote.Quote>> GetQuotesToCustomer(string rfqId)
        {
            var type = this.Resolve<IQuoteRepository>(QuoteRepository).GetQuotesToCustomer(rfqId);
            return type;
        }
        /// <summary>
        /// Get Available Suppliers list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetAvailableSuppliersList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>> GetAvailableSuppliersList(GenericPage<MES.DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSuppliersRepository>(RFQSuppliersRepository).GetAvailableSuppliersList(paging);
            return type;
        }
        /// <summary>
        /// Get RFQ Suppliers list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRFQSuppliersList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSuppliers>> GetRFQSuppliersList(GenericPage<MES.DTO.Library.RFQ.RFQ.RfqSupplierSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSuppliersRepository>(RFQSuppliersRepository).GetRFQSuppliersList(paging);
            return type;
        }
        /// <summary>
        /// delete the RFQSupplier data.
        /// </summary>
        /// <param name="RFQSupplierId"></param>
        /// <returns></returns>
        [HttpPostRoute("DeleteRFQSuppliers")]
        public ITypedResponse<bool?> DeleteRFQSuppliers(int Id)
        {
            var type = this.Resolve<IRFQSuppliersRepository>(RFQSuppliersRepository).DeleteRFQSuppliers(Id);
            return type;
        }
        /// <summary>
        /// send RFQ to availble supplier(s).
        /// </summary>
        /// <param name="SupplierIds"></param>
        /// <returns></returns>
        [HttpPostRoute("SendRFQToSuppliers")]
        public ITypedResponse<bool?> SendRFQToSuppliers(MES.DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData)
        {
            var type = this.Resolve<IRFQSuppliersRepository>(RFQSuppliersRepository).SendRFQToSuppliers(rfqSupplierData);
            return type;
        }
        /// <summary>
        /// send RFQ to rfq supplier(s).
        /// </summary>
        /// <param name="RfqSupplierIds"></param>
        /// <returns></returns>
        [HttpPostRoute("ResendRFQToRfqSuppliers")]
        public ITypedResponse<bool?> ResendRFQToRfqSuppliers(MES.DTO.Library.RFQ.RFQ.RFQSuppliers rfqSupplierData)
        {
            var type = this.Resolve<IRFQSuppliersRepository>(RFQSuppliersRepository).ResendRFQToRfqSuppliers(rfqSupplierData);
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
            var type = this.Resolve<IRFQSuppliersRepository>(RFQSuppliersRepository).SendEmail(EmailData);
            return type;
        }
        /// <summary>
        /// send email.
        /// </summary>
        /// <param name="emailIdsList"></param>
        /// <returns></returns>
        [HttpPostRoute("SendRFQCloseOutEmail")]
        public ITypedResponse<bool?> SendRFQCloseOutEmail(MES.DTO.Library.Common.EmailData EmailData)
        {
            var type = this.Resolve<IRFQRepository>(RFQRepository).SendRFQCloseOutEmail(EmailData);
            return type;
        }
        #endregion
    }
}