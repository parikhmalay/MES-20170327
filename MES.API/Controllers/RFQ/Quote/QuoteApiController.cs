using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NPE.Core;
using NPE.Core.Extended;

using MES.Business.Repositories.RFQ.Quote;
namespace MES.API.Controllers.RFQ.Quote
{
    [AdminPrefix("QuoteApi")]
    public class QuoteApiController : SecuredApiControllerBase
    {
        [Inject]
        public IQuoteRepository QuoteRepository { get; set; }
        public IQuoteDetailsRepository QuoteDetailsRepository { get; set; }
        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> Get(string Id)
        {
            var type = this.Resolve<IQuoteRepository>(QuoteRepository).FindById(Id);
            return type;
        }
        /// <summary>
        /// Get RFQ list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetQuoteList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.Quote.Quote>> GetQuoteList(GenericPage<MES.DTO.Library.RFQ.Quote.SearchCriteria> paging)
        {
            var type = this.Resolve<IQuoteRepository>(QuoteRepository).GetQuoteList(paging);
            return type;
        }
        /// <summary>
        /// Get PartToQuote list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetPartsToQuote")]
        public ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> GetPartsToQuote(string rfqId)
        {
            var type = this.Resolve<IQuoteDetailsRepository>(QuoteDetailsRepository).GetPartsToQuote(rfqId);
            return type;
        }
        /// <summary>
        /// Get GetQuotePartsDetail List.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetQuotePartsDetail")]
        public ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> GetQuotePartsDetail(MES.DTO.Library.RFQ.Quote.Quote qItem)
        {
            var type = this.Resolve<IQuoteDetailsRepository>(QuoteDetailsRepository).GetQuotePartsDetail(qItem);
            return type;
        }
        /// <summary>
        /// Get QuoteDetails List.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetQuoteDetails")]
        public ITypedResponse<MES.DTO.Library.RFQ.Quote.Quote> GetQuoteDetails(string quoteId, bool isR)
        {
            var type = this.Resolve<IQuoteDetailsRepository>(QuoteDetailsRepository).GetQuoteDetails(quoteId, isR);
            return type;
        }
        /// <summary>
        /// Get QuoteDetails List.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetSupplierQuotedDetails")]
        public ITypedResponse<MES.DTO.Library.RFQ.Quote.QuoteDetails> GetSupplierQuotedDetails(MES.DTO.Library.RFQ.Quote.QuoteDetails qdItem)
        {
            var type = this.Resolve<IQuoteDetailsRepository>(QuoteDetailsRepository).GetSupplierQuotedDetails(qdItem);
            return type;
        }

        /// <summary>
        /// Export to Excel.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("exportToExcel")]
        public ITypedResponse<bool?> exportToExcel(MES.DTO.Library.RFQ.Quote.Quote quote)
        {
            var type = this.Resolve<IQuoteRepository>(QuoteRepository).exportToExcel(quote);
            return type;
        }
        /// <summary>
        /// save the Quote data.
        /// </summary>
        /// <param name="RFQ"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<string> Save(MES.DTO.Library.RFQ.Quote.Quote quoteData)
        {
            var type = this.Resolve<IQuoteRepository>(QuoteRepository).Save(quoteData);
            return type;
        }

        /// <summary>
        /// save the Quote history data.
        /// </summary>
        [HttpPostRoute("SaveQuoteCalculationHistory")]
        public ITypedResponse<string> SaveQuoteCalculationHistory(MES.DTO.Library.RFQ.Quote.Quote quoteData)
        {
            var type = this.Resolve<IQuoteRepository>(QuoteRepository).SaveQuoteCalculationHistory(quoteData);
            return type;
        }

    }
}