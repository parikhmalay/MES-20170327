using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQdqRawMaterial : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> RFQSupplierPartDQId { get; set; }
        public string RawMaterialDesc { get; set; }
        public string RawMaterialIndexUsed { get; set; }
        public Nullable<decimal> RawMatInputInKg { get; set; }
        public Nullable<decimal> RawMatCostPerKg { get; set; }
        public Nullable<decimal> RawMatTotal { get; set; }
        public Nullable<decimal> MfgRejectRate { get; set; }
        public Nullable<decimal> MaterialLoss { get; set; }
    }
}
