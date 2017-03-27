using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQParts : CreateEditPropBase
    {
        public int? Id { get; set; }
        public string RfqId { get; set; }
        public string CustomerPartNo { get; set; }
        public string PartDescription { get; set; }
        public string AdditionalPartDesc { get; set; }
        public string RevLevel { get; set; }
        public int? EstimatedQty { get; set; }
        public string MaterialType { get; set; }
        public decimal? PartWeightKG { get; set; }

        public List<RFQPartAttachment> RfqPartAttachmentList { get; set; }
        public List<RFQPartCostComparision> lstRFQPartCostComparison { get; set; }
    
    }

    public class RFQPartAttachment : CreateEditPropBase
    {
        public int? Id { get; set; }
        public int? RfqPartId { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentDetail { get; set; }
        public string AttachmentPathOnServer { get; set; }
    }
}
