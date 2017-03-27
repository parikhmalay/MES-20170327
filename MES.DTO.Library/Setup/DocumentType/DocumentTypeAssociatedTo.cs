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
    public class DocumentTypeAssociatedTo : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }    
        public string Name { get; set; }
        public int DocumentTypeId { get; set; }
        public int AssociatedToId { get; set; }
        public string AssociatedTo { get; set; }
        public string Description { get; set; }
        #endregion
    }
}