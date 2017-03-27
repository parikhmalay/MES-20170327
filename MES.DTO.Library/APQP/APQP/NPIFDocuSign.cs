using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.APQP.APQP
{
    public class apqpNPIFDocuSign : CreateEditPropBase
    {
        public int Id { get; set; }
        public int APQPItemId { get; set; }
        public string PartNumber{ get; set; }
        public string DocumentId { get; set; }
        public string EnvelopeId { get; set; }
        public string InitialDocumentPath { get; set; }
        public string SignedDocumentPath { get; set; }
        public string Status { get; set; }
        public DateTime? DateOfStatus { get; set; }
        public string VoidedReason { get; set; }

        public DateTime? ApprovalSentDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public List<apqpNPIFDocuSignApprovers> lstNPIFDocuSignApprovers { get; set; }
    }
    public class apqpNPIFDocuSignApprovers
    {
        public int Id { get; set; }
        public short RoutingOrder { get; set; }
        public short DesignationId { get; set; }
        public string UserId { get; set; }
        public string RecipientId { get; set; }
    }
}
