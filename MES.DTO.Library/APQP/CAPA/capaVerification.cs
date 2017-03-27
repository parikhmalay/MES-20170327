using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class capaVerification : CreateEditPropBase
    {
        public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> VerificationDate { get; set; }
    }
}