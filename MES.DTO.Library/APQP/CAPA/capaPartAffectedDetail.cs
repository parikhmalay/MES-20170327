using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class capaPartAffectedDetail : CreateEditPropBase
    {
        public int Id { get; set; }
        public int CorrectiveActionId { get; set; }
        public Nullable<int> DefectTypeId { get; set; }
        public Nullable<int> APQPItemId { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public Nullable<System.DateTime> ActualCompletedDate { get; set; }
        public Nullable<int> CustomerRejectedPartQty { get; set; }
        public Nullable<int> SupplierRejectedPartQty { get; set; }
        public Nullable<int> PartsDeliveryLateQty { get; set; }
        public bool IsDeletedFromObject { get; set; }
    }
}