using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class QuotePartItem : CreateEditPropBase
    {
        public int Id { get; set; }
        public string QuoteId { get; set; }
        public string QuoteNumber { get; set; }
        public string RFQId { get; set; }
        public string RFQDate { get; set; }
        public int PartId { get; set; }
        public string CustomerPartNo { get; set; }
        public string PartDescription { get; set; }
        public string CustomerName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Nullable<bool> IsAPQPItem { get; set; }
        public string RevLevel { get; set; }
    }
}
