using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class SAPItem : CreateEditPropBase
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ProjectName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public Nullable<bool> IsAPQPItem { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public int APQPSAPItemId { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string RFQDate { get; set; }
    }
}
