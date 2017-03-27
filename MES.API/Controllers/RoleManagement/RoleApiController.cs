using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NPE.Core;
using NPE.Core.Extended;
using MES.Business.Repositories.RoleManagement;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("RolesApi")]
    public class RoleApiController : SecuredApiControllerBase
    {
        [Inject]
        public IRoleRepository RoleRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.RoleManagement.Role> Get(int Id)
        {
            var type = this.Resolve<IRoleRepository>(RoleRepository).FindById(Id == 0 ? (int?)null : Id);
            return type;
        }
        /// <summary>
        /// save the role data.
        /// </summary>
        /// <param name="Customers"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.RoleManagement.Role role)
        {
            var type = this.Resolve<IRoleRepository>(RoleRepository).Save(role);
            return type;
        }
        #endregion
        /// <summary>
        /// delete the role Item.
        /// </summary>
        /// <param name="CustomersId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int Id)
        {
            var type = this.Resolve<IRoleRepository>(RoleRepository).Delete(Id);
            return type;
        }

        /// <summary>
        /// Get Role list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRoleList")]
        public ITypedResponse<List<MES.DTO.Library.RoleManagement.Role>> GetRoleList(GenericPage<MES.DTO.Library.RoleManagement.SearchCriteria> paging)
        {
            var type = this.Resolve<IRoleRepository>(RoleRepository).GetRoleList(paging);
            return type;
        }
        /// <summary>
        /// Checks if rolename exists.
        /// </summary>
        /// <param name="roleName">Name of the user.</param>
        /// <returns></returns>
        [HttpGetRoute("RoleNameExists")]
        public ITypedResponse<bool?> RoleNameExists(string roleName)
        {
            return this.Resolve<IRoleRepository>(RoleRepository).RoleNameExists(roleName);
        }
    }
}