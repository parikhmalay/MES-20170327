using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.TriggerPoint
{
    public class TriggerPointUsers : CreateEditPropBase
    {
        #region Model property
        public int TriggerPointUserId { get; set; }
        public string Name { get; set; }
        public int TriggerPointId { get; set; }
        public string Id { get; set; }
        #endregion
    }
}