using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.RFQPriority;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("RFQPriorityApi")]
    public class RFQPriorityApiController : SecuredApiControllerBase
    {

        [Inject]
        public IRFQPriorityRepository RFQPriorityRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// RFQPriority list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRFQPrioritys")]
        public ITypedResponse<List<MES.DTO.Library.Setup.RFQPriority.RFQPriority>> GetRFQPrioritys(GenericPage<string> page)
        {
            var type = this.Resolve<IRFQPriorityRepository>(RFQPriorityRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get RFQPrioritys list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRFQPriorityList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.RFQPriority.RFQPriority>> GetRFQPriorityList(GenericPage<MES.DTO.Library.Setup.RFQPriority.SearchCriteria> paging)
        {
            var type = this.Resolve<IRFQPriorityRepository>(RFQPriorityRepository).GetRFQPrioritys(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.RFQPriority.RFQPriority> Get(int Id)
        {
            var type = this.Resolve<IRFQPriorityRepository>(RFQPriorityRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the RFQPriority data.
        /// </summary>
        /// <param name="RFQPriority"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.RFQPriority.RFQPriority RFQPriority)
        {
            var type = this.Resolve<IRFQPriorityRepository>(RFQPriorityRepository).Save(RFQPriority);
            return type;
        }
        /// <summary>
        /// delete the RFQPriority data.
        /// </summary>
        /// <param name="RFQPriorityId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int RFQPriorityId)
        {
            var type = this.Resolve<IRFQPriorityRepository>(RFQPriorityRepository).Delete(RFQPriorityId);
            return type;
        }

        #endregion
    }
}
