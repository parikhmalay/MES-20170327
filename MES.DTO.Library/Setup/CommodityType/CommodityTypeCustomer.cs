using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MES.DTO.Library.Base.Messaging.Resources;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.Setup.CommodityType
{
    public class CommodityTypeCustomer : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        public string Name { get; set; }
        public int CommodityTypeId { get; set; }
        public int CustomerId { get; set; }
        #endregion
    }
}