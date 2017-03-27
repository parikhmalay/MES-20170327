using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.RFQSource;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("RFQSourceApi")]
    public class RFQSourceApiController : SecuredApiControllerBase
    {
        
        [Inject]
        public IRFQSourceRepository RFQSourceRepository { get; set; }
            
        #region Methods
       

        /// <summary>
        /// Get
        /// 
        /// RFQSource list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRFQSources")]
        public ITypedResponse<List<MES.DTO.Library.Setup.RFQSource.RFQSource>> GetRFQSources(GenericPage<string> page)
        {
            var type = this.Resolve<IRFQSourceRepository>(RFQSourceRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get RfqTypes list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRfqSourceList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.RFQSource.RFQSource>> GetRfqSourceList(GenericPage<MES.DTO.Library.Setup.RFQSource.SearchCriteria> paging)
        {
            var type = this.Resolve<IRFQSourceRepository>(RFQSourceRepository).GetRFQSources(paging);
            return type;
        }

        
        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.RFQSource.RFQSource> Get(int Id)
        {
            var type = this.Resolve<IRFQSourceRepository>(RFQSourceRepository).FindById(Id);
            return type;
        }

      
        /// <summary>
        /// save the rfqSource data.
        /// </summary>
        /// <param name="rfqSource"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.RFQSource.RFQSource rfqSource)
        {
            var type = this.Resolve<IRFQSourceRepository>(RFQSourceRepository).Save(rfqSource);
            return type;
        }
        /// <summary>
        /// delete the rfqSource data.
        /// </summary>
        /// <param name="rfqSourceId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int rfqSourceId)
        {
            var type = this.Resolve<IRFQSourceRepository>(RFQSourceRepository).Delete(rfqSourceId);
            return type;
        }
       
        #endregion
    }
}