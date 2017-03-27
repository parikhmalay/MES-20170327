using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class PPAPSubmission : CreateEditPropBase
    {
        public Nullable<int> Id { get; set; }
        public Nullable<int> APQPItemId { get; set; }
        public Nullable<System.DateTime> PSWDate { get; set; }
        public Nullable<System.DateTime> ActualPSWDate { get; set; }
        public string PPAPStatus { get; set; }
        public string MESWarehouse { get; set; }
        public Nullable<System.DateTime> PartInfoEnteredIntoSAPDate { get; set; }
        public Nullable<System.DateTime> PartInfoEnteredIntoPPEPDate { get; set; }
        public Nullable<System.DateTime> PPAPPartsApprovedDate { get; set; }
        public string PackagingDataFilePath { get; set; }
        public string PSWFilePath { get; set; }
        public Nullable<System.DateTime> PSWFileCreatedDate { get; set; }
        public string Comments { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string ProjectName { get; set; }
        public string CustomerName { get; set; }
        public string SupplierName { get; set; }
        public string PartNumber { get; set; }
        public string PartDesc { get; set; }
        public Nullable<int> APQPStatusId { get; set; }
        public string APQPStatus { get; set; }
        public string DrawingNumber { get; set; }
        public string RevLevel { get; set; }
        public Nullable<System.DateTime> ActualSampleShipmentDate { get; set; }
        public string DaysOld { get; set; }
        public string PNDaysOld { get; set; }
        public int IsDocument { get; set; }
    }
}
