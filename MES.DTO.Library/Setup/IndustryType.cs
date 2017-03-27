using MES.DTO.Library.Base.Messaging.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPE.Core;

namespace MES.DTO.Library.Setup.IndustryType
{
    public class IndustryType : IId
    {

        #region Modal property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "IndustryTypeRequired"),
         StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "IndustryTypeLength")]        
        public string industryType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }


        #endregion
    }

    public class SearchCriteria
    {
        public string industrytype { get; set; }
    }
}
