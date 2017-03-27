using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Status;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("StatusApi")]
    public class StatusApiController : SecuredApiControllerBase
    {

        [Inject]
        public IStatusRepository StatusRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// Status list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetStatus")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Status.Status>> GetStatus(GenericPage<string> page)
        {
            var type = this.Resolve<IStatusRepository>(StatusRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get Statuss list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetStatusList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Status.Status>> GetStatusList(GenericPage<MES.DTO.Library.Setup.Status.SearchCriteria> paging)
        {
            var type = this.Resolve<IStatusRepository>(StatusRepository).GetStatus(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.Status.Status> Get(int Id)
        {
            var type = this.Resolve<IStatusRepository>(StatusRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the status data.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Status.Status status)
        {
            var type = this.Resolve<IStatusRepository>(StatusRepository).Save(status);
            return type;
        }
        /// <summary>
        /// delete the status data.
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int statusId)
        {
            var type = this.Resolve<IStatusRepository>(StatusRepository).Delete(statusId);
            return type;
        }

        #endregion
    }
}
