using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.TriggerPoint;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("TriggerPointApi")]
    public class TriggerPointApiController : SecuredApiControllerBase
    {
        [Inject]
        public ITriggerPointRepository TriggerPointRepository { get; set; }

        #region Methods

        /// <summary>
        /// save the TriggerPoint data.
        /// </summary>
        /// <param name="triggerPoint"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.TriggerPoint.TriggerPoint triggerPoint)
        {
            var type = this.Resolve<ITriggerPointRepository>(TriggerPointRepository).Save(triggerPoint);
            return type;
        }

        /// <summary>
        /// delete the TriggerPoint data.
        /// </summary>
        /// <param name="triggerPointId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int triggerPointId)
        {
            var type = this.Resolve<ITriggerPointRepository>(TriggerPointRepository).Delete(triggerPointId);
            return type;
        }
       
        /// <summary>
        /// Get TriggerPoint list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetTriggerPointsList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.TriggerPoint.TriggerPoint>> GetTriggerPointsList(GenericPage<MES.DTO.Library.Setup.TriggerPoint.SearchCriteria> paging)
        {
            var type = this.Resolve<ITriggerPointRepository>(TriggerPointRepository).GetTriggerPointsList(paging);
            return type;
        }

        #endregion Methods
    }
}