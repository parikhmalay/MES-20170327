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
namespace MES.API.Controllers.RFQ.RFQ
{
    [AdminPrefix("RFQSupplierQuoteApi")]
    public class RFQSupplierQuoteApiController : SecuredApiControllerBase
    {
        [Inject]
        public IRFQSupplierPartQuoteRepository RFQSupplierPartQuoteRepository { get; set; }
        #region "Submit Detail quote simplified"

        /// <summary>
        /// save Supplier Quote List data.
        /// </summary>
        /// <param name="RFQSupplierPartQuoteList"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveSupplierQuoteList")]
        public ITypedResponse<bool?> SaveSupplierQuoteList(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).SaveSupplierQuoteList(RFQSupplierPartQuoteList);
            return type;
        }

        /// <summary>
        /// Get Supplier Quote list.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("GetSupplierQuoteList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetSupplierQuoteList(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).GetSupplierQuoteList(paging);
            return type;
        }

        /// <summary>
        /// Export to Excel.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("exportToExcelSupplierQuote")]
        public ITypedResponse<bool?> exportToExcelSupplierQuote(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).exportToExcelSupplierQuote(paging);
            return type;
        }

        /// <summary>
        /// Get RFQ details.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("getRFQDetails")]
        public ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQ> getRFQDetails(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).getRFQDetails(paging);
            return type;
        }
        /// <summary>
        /// Export to Excel.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("DownloadRfqSupplierPartQuote")]
        public ITypedResponse<bool?> DownloadRfqSupplierPartQuote(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).DownloadRfqSupplierPartQuote(paging);
            return type;
        }

        /// <summary>
        /// upload rspq.
        /// </summary>
        /// <param name="RFQ"></param>
        /// <returns></returns>
        [HttpPostRoute("UploadRfqSupplierPartQuote")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> UploadRfqSupplierPartQuote(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).UploadRfqSupplierPartQuote(paging);
            return type;
        }
        #endregion


        /// <summary>
        /// Get RFQ Part Quote details.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        [HttpPostRoute("getRFQPartCostComparisonList")]
        public ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQ> getRFQPartCostComparisonList(string rfqId)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).GetRFQPartCostComparisonList(rfqId);
            return type;
        }

    }
}