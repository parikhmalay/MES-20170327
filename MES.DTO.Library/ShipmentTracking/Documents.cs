using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.ShipmentTracking
{
    public class Documents : CreateEditPropBase
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }
        public string DocumentFilePath { get; set; }
        public string DocumentFileName { get; set; }
        public MES.DTO.Library.Common.LookupFields DocumentTypeItem { get; set; }
    }
}
