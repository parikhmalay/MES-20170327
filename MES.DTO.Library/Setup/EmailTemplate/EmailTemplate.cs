using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.EmailTemplate
{
    public class EmailTemplate : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "TitleRequired"), StringLength(50, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "TitleLength")]
        public string Title { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "EmailSubjectRequired"), StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "EmailSubjectLength")]
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string ShortCode { get; set; }
        public string Description { get; set; }
        public string TestEmailAddress { get; set; }
        #endregion
    }

    public class SearchCriteria
    {
        public string Title { get; set; }
        public string EmailSubject { get; set; }
    }
}
