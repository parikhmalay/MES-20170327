using MES.DTO.Library.RoleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RoleManagement
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public int DefaultObjectId { get; set; }
        public string ObjectName { get; set; }
        public List<RoleObjectPrivilege> lstRoleObjectPrivilege { get; set; }
    }
    public class SearchCriteria
    {
        public int RoleId { get; set; }
        public bool? Active { get; set; }
    }
}
