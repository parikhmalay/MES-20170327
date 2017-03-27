using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.ProjectStage
{
    public class ProjectStage : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "ProjectStageRequired"), 
        StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "ProjectStageLength")]
        public string projectStage { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "ProjectCategoryRequired")]
        public int? ProjectCategoryId { get; set; }
        public string ProjectCategory { get; set; }
        #endregion
    }

    public class SearchCriteria
    {
        public string projectStage { get; set; }
        public int? ProjectCategoryId { get; set; }
    }
}
