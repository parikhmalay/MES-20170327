using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.ProjectCategory
{
    public class ProjectCategory : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "ProjectCategoryRequired"), StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "ProjectCategoryLength")]
        public string projectCategory { get; set; }
        #endregion
    }

    public class SearchCriteria
    {
        public string projectCategory { get; set; }
    }
}
