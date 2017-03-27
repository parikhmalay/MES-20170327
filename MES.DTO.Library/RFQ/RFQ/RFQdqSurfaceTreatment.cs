using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQdqSurfaceTreatment : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> RFQSupplierPartDQId { get; set; }
        public Nullable<int> CoatingTypeId { get; set; }
        public string CoatingType { get; set; }
        public Nullable<int> PartsPerHour { get; set; }
        public Nullable<decimal> ManPlusMachineRatePerHour { get; set; }
        public Nullable<decimal> CoatingCostPerHour { get; set; }
    }
}
