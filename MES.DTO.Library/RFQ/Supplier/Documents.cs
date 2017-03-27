using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.Supplier
{
    public class Documents : CreateEditPropBase
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string DocumentFilePath { get; set; }
        public string DocumentFileName { get; set; }
        public string Comment { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public Nullable<int> DocumentTypeId { get; set; }

        public LookupFields DocumentTypeItem { get; set; }
    }
}
