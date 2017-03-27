using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class capaApproval : CreateEditPropBase
    {
         public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public Nullable<short> TitleId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
    }
}