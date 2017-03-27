using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.Destination
{
    public class Destination : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "DestinationRequired"),
        StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "DestinationLength")]
        public string destination { get; set; }
        [StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "LocationLength")]
        public string Location { get; set; }
        public bool IsWarehouse { get; set; }
        #endregion

        #region Extension methods
        //public override bool Equals(object obj)
        //{
        //    if (obj == null) return false;
        //    if (obj.GetType() != this.GetType()) return false;
        //    MES.DTO.Library.Setup.Destination.Destination at = obj as MES.DTO.Library.Destination.Destination;
        //    return (at.CreatedBy == this.CreatedBy);
        //} 
        #endregion
    }

    public class SearchCriteria
    {
        public string destination { get; set; }
        public string Location { get; set; }
        public bool? IsWarehouse { get; set; }
    }
}
