using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQdqPrimaryProcessConversion : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> RFQSupplierPartDQId { get; set; }
        public Nullable<int> MachineDescId { get; set; }
        public string MachineDescription { get; set; }
        public Nullable<int> MachineSize { get; set; }
        public Nullable<int> CycleTime { get; set; }
        public Nullable<decimal> ManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> ProcessConversionCostPerPart { get; set; }
    }
}
