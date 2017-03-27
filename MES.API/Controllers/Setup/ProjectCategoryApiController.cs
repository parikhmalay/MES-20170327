using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.ProjectCategory;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("ProjectCategoryApi")]
    public class ProjectCategoryApiController : SecuredApiControllerBase
    {
        [Inject]
        public IProjectCategoryRepository ProjectCategoryRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.ProjectCategory.ProjectCategory> Get(int Id)
        {
            var type = this.Resolve<IProjectCategoryRepository>(ProjectCategoryRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Project Category data.
        /// </summary>
        /// <param name="ProjectCategory"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.ProjectCategory.ProjectCategory projectCategory)
        {
            var type = this.Resolve<IProjectCategoryRepository>(ProjectCategoryRepository).Save(projectCategory);
            return type;
        }

        /// <summary>
        /// delete the Project Category data.
        /// </summary>
        /// <param name="ProjectCategoryId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int projectCategoryId)
        {
            var type = this.Resolve<IProjectCategoryRepository>(ProjectCategoryRepository).Delete(projectCategoryId);
            return type;
        }

        /// <summary>
        /// Get ProjectCategory list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetProjectCategoryList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.ProjectCategory.ProjectCategory>> GetProjectCategoryList(GenericPage<MES.DTO.Library.Setup.ProjectCategory.SearchCriteria> paging)
        {
            var type = this.Resolve<IProjectCategoryRepository>(ProjectCategoryRepository).GetProjectCategoryList(paging);
            return type;
        }
        #endregion Methods
    }
}