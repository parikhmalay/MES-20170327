using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQPartCostComparision
    {
        public Nullable<DateTime> UpdatedDate { get; set; }
        public decimal PiecePrice { get; set; }
        public Nullable<decimal> ToolingCost { get; set; }
        public decimal SupplierCostPerKg { get; set; }
        public int SupplierId { get; set; }
        public int rfqPartId { get; set; }
        public bool rdoSelect { get; set; }
        public string rdoSelectValue { get; set; }
    }
}
