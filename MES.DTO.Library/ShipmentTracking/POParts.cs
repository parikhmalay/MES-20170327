using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.ShipmentTracking
{
    public class POParts : CreateEditPropBase
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public string PONumber { get; set; }
        public string PartNumber { get; set; }
        public int PartQuantity { get; set; }
        public string PartDescription { get; set; }
    }
}
