using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQdqMachiningOtherOperation : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> RFQSupplierPartDQId { get; set; }
        public Nullable<int> SecondaryOperationDescId { get; set; }
        public string SecondaryOperationDescription { get; set; }
        public Nullable<int> CycleTime { get; set; }
        public Nullable<decimal> ManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> SecondaryCostPerPart { get; set; }
    }
}
