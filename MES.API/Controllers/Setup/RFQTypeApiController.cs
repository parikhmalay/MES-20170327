using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.RFQType;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("RFQTypeApi")]
    public class RFQTypeApiController : SecuredApiControllerBase
    {
        
        [Inject]
        public IRFQTypeRepository RFQTypeRepository { get; set; }
            
        #region Methods
       

        /// <summary>
        /// Get
        /// 
        /// RFQType list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRFQTypes")]
        public ITypedResponse<List<MES.DTO.Library.Setup.RFQType.RFQType>> GetRFQTypes(GenericPage<string> page)
        {
            var type = this.Resolve<IRFQTypeRepository>(RFQTypeRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get RfqTypes list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRfqTypeList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.RFQType.RFQType>> GetRfqTypeList(GenericPage<MES.DTO.Library.Setup.RFQType.SearchCriteria> paging)
        {
            var type = this.Resolve<IRFQTypeRepository>(RFQTypeRepository).GetRFQTypes(paging);
            return type;
        }

        
        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.RFQType.RFQType> Get(int Id)
        {
            var type = this.Resolve<IRFQTypeRepository>(RFQTypeRepository).FindById(Id);
            return type;
        }

      
        /// <summary>
        /// save the rfqType data.
        /// </summary>
        /// <param name="rfqType"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.RFQType.RFQType rfqType)
        {
            var type = this.Resolve<IRFQTypeRepository>(RFQTypeRepository).Save(rfqType);
            return type;
        }
        /// <summary>
        /// delete the rfqType data.
        /// </summary>
        /// <param name="rfqTypeId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int rfqTypeId)
        {
            var type = this.Resolve<IRFQTypeRepository>(RFQTypeRepository).Delete(rfqTypeId);
            return type;
        }
       
        #endregion
    }
}