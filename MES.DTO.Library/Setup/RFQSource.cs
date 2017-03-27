using MES.DTO.Library.Base.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.RFQSource
{
    public class RFQSource : CreateEditPropBase, IId
    {

        #region Modal property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "RFQSourceRequired"),
        StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "RFQSourceLength")]

        public string rfqSource { get; set; }


        #endregion
    }

    public class SearchCriteria
    {
        public string rfqSource { get; set; }
    }
}
