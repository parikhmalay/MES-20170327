using MES.DTO.Library.Identity;

using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.UserManagement
{
    public interface IUserManagementRepository : ICrudMethods<LoginUser, String, String, LoginUser, String, bool?, String, LoginUser>
    {
        ITypedResponse<bool?> ChangePassword(MES.DTO.Library.UserManagement.ChangePassword user);

        ITypedResponse<List<LoginUser>> GetUsers(IPage<SearchCriteria> paging);

        ITypedResponse<bool?> UserNameExists(string userName);

        ITypedResponse<LoginUser> GetCurrentUserInfo();
        ITypedResponse<SearchCriteria> DefaultSearchCriteria();
    }
}

