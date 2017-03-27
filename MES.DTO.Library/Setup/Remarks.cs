using MES.DTO.Library.Base.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.Remarks
{
    public class Remarks : CreateEditPropBase,IId
    {

        #region Modal property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "RemarksRequired"),
        StringLength(200, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "RemarksLength")]
        public string remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        //public string UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        public List<RemarksAssociatedTo> RemarksAssociatedToList { get; set; }
        #endregion
    }

    public class SearchCriteria
    {
        public string remarks { get; set; }
        public int? AssociatedToId { get; set; }
    }
}
