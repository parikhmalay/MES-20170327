using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RoleManagement
{
    public class RoleObjectPrivilege
    {
        public int? Id { get; set; }
        public int RoleId { get; set; }
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }
        public int? PrivilegeId { get; set; }
        public int? ParentId { get; set; }
        public int? Parent { get; set; }
        public string Privilege { get; set; }
        public string PageName { get; set; }
        public string MenuClass { get; set; }
        public bool HasChild { get; set; }
        public bool isPublicPage { get; set; }
        public bool isMenuItem { get; set; }
        public bool isActive { get; set; }
        public bool HasPricingFieldsAccess { get; set; }
        public bool AllowDeleteRecord { get; set; }
        public bool AllowConfidentialDocumentType { get; set; }
        public bool AllowExportToSAP { get; set; }
        public bool AllowSendDataToSAP { get; set; }
        public bool AllowCheckNPIFStatus { get; set; }

        public List<RoleObjectPrivilege> childObject { get; set; }
    }
}
