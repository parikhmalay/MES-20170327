using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.TriggerPoint
{
    public class TriggerPoint : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "TriggerPointRequired"),
        StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "TriggerPointLength")]
        public string triggerPoint { get; set; }
        public List<TriggerPointUsers> TriggerPointUsersList { get; set; }
        #endregion
    }
    public class SearchCriteria
    {
        public string triggerPoint { get; set; }
        public string UserId { get; set; }
    }
}
