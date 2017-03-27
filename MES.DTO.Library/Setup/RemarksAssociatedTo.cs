using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.Remarks
{
    public class RemarksAssociatedTo : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }    // this id is not primary key for this table, it will work as associated toId because of dictionary object in UI. 
        public string Name { get; set; }
        public int RemarksId { get; set; }
        public int AssociatedToId { get; set; }
        public string AssociatedTo { get; set; }
        public string Description { get; set; }
        #endregion
    }
}