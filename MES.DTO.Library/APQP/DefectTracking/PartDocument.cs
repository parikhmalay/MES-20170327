using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.DefectTracking
{
    public class PartDocument : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> DefectTrackingDetailId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public bool IsConfidential { get; set; }
        public string FileTitle { get; set; }
        public string FilePath { get; set; }
        public string Comments { get; set; }
        public string RevLevel { get; set; }
        public Nullable<int> AssociatedToId { get; set; }
        public string SectionName { get; set; }
    }
}
