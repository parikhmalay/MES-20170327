using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class capaTempCountermeasure : CreateEditPropBase
    {
        public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
    }
}