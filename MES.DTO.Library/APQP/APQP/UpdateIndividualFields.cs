using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.APQP.APQP
{
    public class UpdateIndividualFields
    {
        public string DrawingNumber { get; set; }
        public string RevLevel { get; set; }
        public int? StatusId { get; set; }
        public string FieldName { get; set; }
        public string UpdatedFromSource { get; set; }
        public int APQPItemId { get; set; }
    }
}
