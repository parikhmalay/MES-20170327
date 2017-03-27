using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQdqOverhead : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> RFQSupplierPartDQId { get; set; }
        public Nullable<decimal> InventoryCarryingCost { get; set; }
        public Nullable<decimal> Packing { get; set; }
        public Nullable<decimal> LocalFreightToPort { get; set; }
        public Nullable<decimal> ProfitAndSGA { get; set; }
        public Nullable<decimal> OverheadPercentPiecePrice { get; set; }
        public Nullable<decimal> PackagingMaterial { get; set; }

    }
}
