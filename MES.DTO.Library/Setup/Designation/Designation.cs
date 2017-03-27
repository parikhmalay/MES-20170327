using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.Designation
{
    public class Designation : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "DesignationRequired"), StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "DesignationLength")]
        public string designation { get; set; }
        public bool? IsSystemDefault { get; set; }
        #endregion
    }

    public class SearchCriteria
    {
        public string designation { get; set; }
    }
}
