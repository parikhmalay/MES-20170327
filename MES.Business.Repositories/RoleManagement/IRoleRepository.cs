using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.RoleManagement
{
    public interface IRoleRepository : ICrudMethods<MES.DTO.Library.RoleManagement.Role, int?, string,
        MES.DTO.Library.RoleManagement.Role, int, bool?, int?, MES.DTO.Library.RoleManagement.Role>
    {
        NPE.Core.ITypedResponse<List<DTO.Library.RoleManagement.Role>> GetRoleList(NPE.Core.IPage<DTO.Library.RoleManagement.SearchCriteria> paging);        
        ITypedResponse<bool?> RoleNameExists(string roleName);
    }
}
