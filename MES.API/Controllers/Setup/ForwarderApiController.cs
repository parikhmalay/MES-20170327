using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Forwarder;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("ForwarderApi")]
    public class ForwarderApiController : SecuredApiControllerBase
    {
        [Inject]
        public IForwarderRepository ForwarderRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.Forwarder.Forwarder> Get(int Id)
        {
            var type = this.Resolve<IForwarderRepository>(ForwarderRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Forwarder data.
        /// </summary>
        /// <param name="Forwarder"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Forwarder.Forwarder forwarder)
        {
            var type = this.Resolve<IForwarderRepository>(ForwarderRepository).Save(forwarder);
            return type;
        }

        /// <summary>
        /// delete the forwarder data.
        /// </summary>
        /// <param name="ForwarderId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int forwarderId)
        {
            var type = this.Resolve<IForwarderRepository>(ForwarderRepository).Delete(forwarderId);
            return type;
        }

        /// <summary>
        /// Get Forwarder list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetForwarderList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Forwarder.Forwarder>> GetForwarderList(GenericPage<MES.DTO.Library.Setup.Forwarder.SearchCriteria> paging)
        {
            var type = this.Resolve<IForwarderRepository>(ForwarderRepository).GetForwarderList(paging);
            return type;
        }
        #endregion Methods
    }
}