using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.ChangeRequest
{
    public class Document : CreateEditPropBase
    {
        public int Id { get; set; }
        public int ChangeRequestId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public bool IsConfidential { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
        public string Comments { get; set; }
        public Nullable<int> AuditLogId { get; set; }

        public LookupFields DocumentTypeItem { get; set; }
    }
}
