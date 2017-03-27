using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class PredefinedDocumentTypes : CreateEditPropBase
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public Nullable<bool> IsConfidential { get; set; }
        public string AssociatedToIds { get; set; }
        public string AssociatedToName { get; set; }
        public string DocumentTypeUsedAssociatedIds { get; set; }
    }
}
