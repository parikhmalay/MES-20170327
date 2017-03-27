using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.ProjectStage;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("ProjectStageApi")]
    public class ProjectStageApiController : SecuredApiControllerBase
    {
        [Inject]
        public IProjectStageRepository ProjectStageRepository { get; set; }

        #region Methods

        /// <summary>
        /// save the Project Stage data.
        /// </summary>
        /// <param name="projectStage"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.ProjectStage.ProjectStage projectStage)
        {
            var type = this.Resolve<IProjectStageRepository>(ProjectStageRepository).Save(projectStage);
            return type;
        }

        /// <summary>
        /// delete the ProjectStage data.
        /// </summary>
        /// <param name="projectStageId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int projectStageId)
        {
            var type = this.Resolve<IProjectStageRepository>(ProjectStageRepository).Delete(projectStageId);
            return type;
        }
       
        /// <summary>
        /// Get ProjectStage list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetProjectStagesList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.ProjectStage.ProjectStage>> GetProjectStagesList(GenericPage<MES.DTO.Library.Setup.ProjectStage.SearchCriteria> paging)
        {
            var type = this.Resolve<IProjectStageRepository>(ProjectStageRepository).GetProjectStagesList(paging);
            return type;
        }

        #endregion Methods
    }
}