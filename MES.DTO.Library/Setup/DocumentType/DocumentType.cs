using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.DocumentType
{
    public class DocumentType : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "DocumentTypeRequired"),
        StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "DocumentTypeLength")]
        public string documentType { get; set; }
        public bool? IsConfidential { get; set; }
        public List<DocumentTypeAssociatedTo> DocumentTypeAssociatedToList { get; set; }
        #endregion
    }
    public class SearchCriteria
    {
        public string documentType { get; set; }
        public bool? IsConfidential { get; set; }
        public int? AssociatedToId { get; set; }
    }
}
