using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RoleManagement
{
    public class SecurityObjectInfo
    {
        public List<RoleObjectPrivilege> childObject { get; set; }
        public int ObjectId
        {
            get;
            set;
        }
        public bool IsMenuItem
        {
            get;
            set;
        }
        public string ObjectName
        {
            get;
            set;
        }
        public string PageName
        {
            get;
            set;
        }
        public string Privilege
        {
            get;
            set;
        }

    }
}
