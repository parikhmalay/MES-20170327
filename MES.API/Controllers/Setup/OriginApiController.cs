using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Origin;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("OriginApi")]
    public class OriginApiController : SecuredApiControllerBase
    {

        [Inject]
        public IOriginRepository OriginRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// Origin list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetOrigins")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Origin.Origin>> GetOrigins(GenericPage<string> page)
        {
            var type = this.Resolve<IOriginRepository>(OriginRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get Origins list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetOriginList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Origin.Origin>> GetOriginList(GenericPage<MES.DTO.Library.Setup.Origin.SearchCriteria> paging)
        {
            var type = this.Resolve<IOriginRepository>(OriginRepository).GetOrigins(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.Origin.Origin> Get(int Id)
        {
            var type = this.Resolve<IOriginRepository>(OriginRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the origin data.
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Origin.Origin origin)
        {
            var type = this.Resolve<IOriginRepository>(OriginRepository).Save(origin);
            return type;
        }
        /// <summary>
        /// delete the origin data.
        /// </summary>
        /// <param name="originId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int originId)
        {
            var type = this.Resolve<IOriginRepository>(OriginRepository).Delete(originId);
            return type;
        }

        #endregion
    }
}
