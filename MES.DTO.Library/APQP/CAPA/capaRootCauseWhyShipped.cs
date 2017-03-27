using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class capaRootCauseWhyShipped : CreateEditPropBase
    {
        public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public short QueryId { get; set; }
        public string Reason { get; set; }
    }
}