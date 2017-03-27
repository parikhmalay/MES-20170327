using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class capaPartDocument : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> CorrectiveActionId { get; set; }
        public int PartAffectedDetailsId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
        public string Comments { get; set; }
        public string RevLevel { get; set; }
        public Nullable<int> AssociatedToId { get; set; }
        public bool IsConfidential { get; set; }
        public MES.DTO.Library.Common.LookupFields DocumentTypeItem { get; set; }
    }
}

