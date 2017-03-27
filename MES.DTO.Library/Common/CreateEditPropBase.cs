using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.Common
{
    public class CreateEditPropBase
    {
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class ItemList
    {

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
