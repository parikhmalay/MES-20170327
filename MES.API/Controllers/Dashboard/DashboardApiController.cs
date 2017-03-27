using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Dashboard;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Dashboard
{
    [AdminPrefix("DashboardApi")]
    public class DashboardApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDashboardRepository DashboardRepository { get; set; }

        #region Methods
        [HttpPostRoute("GetDashboardSummary")]
        public ITypedResponse<MES.DTO.Library.Dashboard.Dashboard> GetDashboardSummary(GenericPage<MES.DTO.Library.Dashboard.SearchCriteria> paging)
        {
            var type = this.Resolve<IDashboardRepository>(DashboardRepository).GetDashboardSummary(paging);
            return type;
        }
        #endregion
    }
}
