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
using MES.Business.Repositories.UserManagement;

namespace MES.API.Controllers.UserManagement
{
    [AdminPrefix("UserIdentityApi")]
    public class UserIdentityApiController : SecuredApiControllerBase
    {
        [Inject]
        public IUserManagementRepository UserManagementRepository { get; set; }
        #region Methods
        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("GetCurrentUserInfo")]
        public ITypedResponse<DTO.Library.Identity.LoginUser> GetCurrentUserInfo()
        {          
            var type = this.Resolve<IUserManagementRepository>(UserManagementRepository).GetCurrentUserInfo();
            return type;
        }


        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="cp">The cp.</param>
        /// <returns></returns>
        [HttpPostRoute("ChangePassword")]
        public ITypedResponse<bool?> ChangePassword(MES.DTO.Library.UserManagement.ChangePassword user)
        {
            return this.Resolve<IUserManagementRepository>(UserManagementRepository).ChangePassword(user);
        }

        /// <summary>
        /// Get User list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetUserList")]
        public ITypedResponse<List<DTO.Library.Identity.LoginUser>> GetUserList(GenericPage<MES.DTO.Library.Identity.SearchCriteria> paging)
        {
            var type = this.Resolve<IUserManagementRepository>(UserManagementRepository).GetUsers(paging);
            return type;
        }

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<DTO.Library.Identity.LoginUser> Get(string Id)
        {
            var type = this.Resolve<IUserManagementRepository>(UserManagementRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the User data.
        /// </summary>
        /// <param name="Users"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<string> Save(DTO.Library.Identity.LoginUser user)
        {
            return this.Resolve<IUserManagementRepository>(UserManagementRepository).Save(user);            
        }

        /// <summary>
        /// delete the User data.
        /// </summary>
        /// <param name="UsersId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(string Id)
        {
            var type = this.Resolve<IUserManagementRepository>(UserManagementRepository).Delete(Id);
            return type;
        }
        /// <summary>
        /// Checks if usernam exists.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        [HttpGetRoute("UserNameExists")]
        public ITypedResponse<bool?> UserNameExists(string userName)
        {
            return this.Resolve(UserManagementRepository).UserNameExists(userName);
        }
        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("DefaultSearchCriteria")]
        public ITypedResponse<MES.DTO.Library.Identity.SearchCriteria> DefaultSearchCriteria()
        {
            var type = this.Resolve<IUserManagementRepository>(UserManagementRepository).DefaultSearchCriteria();
            return type;
        }

        #endregion
    }
}