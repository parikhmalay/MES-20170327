using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.DocumentManagement
{
    public class Document : CreateEditPropBase
    {
        public int Id { get; set; }
        public int APQPItemId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public bool IsConfidential { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public Nullable<System.DateTime> PreparedDate { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
        public string Comments { get; set; }
        public string RevLevel { get; set; }
        public Nullable<long> RowNumber { get; set; }
        public Nullable<int> DocumentCount { get; set; }
        public string CreatedDateString { get; set; }
        public bool chkSelect { get; set; }
    }
}
