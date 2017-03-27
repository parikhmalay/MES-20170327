using MES.DTO.Library.Base.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core;

namespace MES.DTO.Library.Setup.Process
{
    public class Process : IId
    {

        #region Modal property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "ProcessRequired"),
          StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "ProcessLength")]
        public string process { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        //public string UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }


        #endregion
    }

    public class SearchCriteria
    {
        public string process { get; set; }
    }
}
