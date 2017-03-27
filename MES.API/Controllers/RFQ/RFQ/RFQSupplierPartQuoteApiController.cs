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
    [AdminPrefix("RFQSupplierPartQuoteApi")]
    public class RFQSupplierPartQuoteApiController : ApiController
    {
        [Inject]
        public IRFQSupplierPartQuoteRepository RFQSupplierPartQuoteRepository { get; set; }
        #region "Submit Quote Simplified"

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuote> Get(int Id)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save RFQSupplierPartQuote data.
        /// </summary>
        /// <param name="RFQSupplierPartQuote"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveSubmitQuote")]
        public ITypedResponse<bool?> SaveSubmitQuote(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).SaveSubmitQuote(RFQSupplierPartQuoteList);
            return type;
        }
        /// <summary>
        /// save submit no quote data.
        /// </summary>
        /// <param name="Paging"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveSubmitNoQuote")]
        public ITypedResponse<bool?> SaveSubmitNoQuote(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).SaveSubmitNoQuote(paging);
            return type;
        }

        /// <summary>
        /// save the RFQSupplierPartQuote data.
        /// </summary>
        /// <param name="RFQSupplierPartQuote"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuote RFQSupplierPartQuote)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).Save(RFQSupplierPartQuote);
            return type;
        }

        /// <summary>
        /// delete the RFQSupplierPartQuote data.
        /// </summary>
        /// <param name="RFQSupplierPartQuoteId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int RFQSupplierPartQuoteId)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).Delete(RFQSupplierPartQuoteId);
            return type;
        }

        /// <summary>
        /// Get RFQSupplierPartQuote list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRFQSupplierPartQuoteList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetRFQSupplierPartQuoteList(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).GetRFQSupplierPartQuoteList(paging);
            return type;
        }
        #endregion

        #region "Submit Detail quote simplified"
        
        /// <summary>
        /// save RFQSupplierPartQuote data.
        /// </summary>
        /// <param name="RFQSupplierPartQuote"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveSubmitDQ")]
        public ITypedResponse<bool?> SaveSubmitDQ(List<DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ> RFQSupplierPartQuoteList)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).SaveSubmitDQ(RFQSupplierPartQuoteList);
            return type;
        }
        /// <summary>
        /// save submit no DQ data.
        /// </summary>
        /// <param name="Paging"></param>
        /// <returns></returns>
        [HttpPostRoute("SaveSubmitNoDQ")]
        public ITypedResponse<bool?> SaveSubmitNoDQ(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).SaveSubmitNoDQ(paging);
            return type;
        }
        
        /// <summary>
        /// Get DQRFQSupplierPartQuote list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDQRFQSupplierPartQuoteList")]
        public ITypedResponse<List<MES.DTO.Library.RFQ.RFQ.RFQSupplierPartQuoteDQ>> GetDQRFQSupplierPartQuoteList(GenericPage<MES.DTO.Library.RFQ.RFQ.RFQSupplierParQuoteSearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSupplierPartQuoteRepository>(RFQSupplierPartQuoteRepository).GetDQRFQSupplierPartQuoteList(paging);
            return type;
        }
        #endregion
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
    }
}