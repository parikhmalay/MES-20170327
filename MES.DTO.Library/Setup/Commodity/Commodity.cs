using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.Commodity
{
    public class Commodity : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "CommodityRequired"), StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "CommodityLength")]
        public string CommodityName { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "CategoryRequired")]
        public short CategoryId { get; set; }
        public string CategoryName { get; set; }
        #endregion
    }

    public class SearchCriteria
    {
        public string CommodityName { get; set; }
        public short? CategoryId { get; set; }
    }
}
