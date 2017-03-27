using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.APQP
{
    public class APQP : CreateEditPropBase
    {
        public int Id { get; set; }
        public KickOff objKickOff { get; set; }
        public ToolingLaunch objToolingLaunch { get; set; }
        public ProjectTracking objProjectTracking { get; set; }
        public PPAPSubmission objPPAPSubmission { get; set; }
        public dynamic objTooltip { get; set; }
        public bool chkSelect { get; set; }
        public int SelectedClassId { get; set; }

        public bool AllowConfidentialDocumentType { get; set; }
        public bool AllowExportToSAP { get; set; }
        public bool AllowSendDataToSAP { get; set; }
        public bool AllowCheckNPIFStatus { get; set; }
        public bool AllowDeleteRecord { get; set; }
        public bool HasPricingFieldsAccess { get; set; }
    }
    public class SearchCriteria
    {
        public string PartNo { get; set; }
        public string RFQNumber { get; set; }
        public string QuoteNumber { get; set; }
        public string CustomerName { get; set; }
        public string ProjectName { get; set; }
        public string APQPStatusIds { get; set; }
        public string SAMUserId { get; set; }
        public string APQPQualityEngineerId { get; set; }
        public string SupplyChainCoordinatorId { get; set; }
        public bool AllowConfidentialDocumentType { get; set; }
        public string SearchHeading { get; set; }
        //public bool isFirstTimeLoad { get; set; }
        public string SectionName { get; set; }
    }
}
