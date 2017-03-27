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
    public class CommodityType : CreateEditPropBase
    {
        #region Model property
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "CommodityTypeRequired"),
        StringLength(100, ErrorMessageResourceType = typeof(DTOMessageResources), ErrorMessageResourceName = "CommodityTypeLength")]
        public string commodityType { get; set; }
        public List<CommodityTypeCustomer> CommodityTypeCustomerList { get; set; }
        public List<CommodityTypeSupplier> CommodityTypeSupplierList { get; set; }
        #endregion
    }
    public class SearchCriteria
    {
        public string commodityType { get; set; }
        public int? CustomerId { get; set; }
        public int? SupplierId { get; set; }
    }
}
