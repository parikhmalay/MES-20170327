using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class CAPA : CreateEditPropBase
    {
        public int Id { get; set; }
        public Nullable<int> DefectTrackingId { get; set; }
        public string IncludeInPPM { get; set; }
        public string CorrectiveActionType { get; set; }
        public string CorrectiveActionInitiatedBy { get; set; }
        public Nullable<System.DateTime> CorrectiveActionInitiatedDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string SupplierContactName { get; set; }
        public string ReviewCtrlPlan { get; set; }
        public string ReviewFMEA { get; set; }
        public string PartNumber { get; set; }
        public string PartCodes { get; set; }
        public string Supplier { get; set; }
        public string CustomerRejectedPartQty { get; set; }
        public string Mode { get; set; }
        public int AddToDT { get; set; }
        public string SupplierCodeWithName { get; set; }
        public string CustomerCodeWithName { get; set; }

        public List<capaPartAffectedDetail> lstcapaPartAffectedDetails { get; set; }
        public List<capaProblemDescription> lstcapaProblemDescriptions { get; set; }
        public List<capaTempCountermeasure> lstcapaTempCountermeasures { get; set; }
        public List<capaVerification> lstcapaVerifications { get; set; }
        public List<capaRootCauseWhyMade> lstcapaRootCauseWhyMade { get; set; }
        public List<capaRootCauseWhyShipped> lstcapaRootCauseWhyShipped { get; set; }
        public List<capaPermanentCountermeasure> lstcapaPermanentCountermeasures { get; set; }
        public List<capaFeedForward> lstcapaFeedForwards { get; set; }
        public List<capaApproval> lstcapaApprovals { get; set; }
    }
    public class SearchCriteria
    {
        public string CorrectiveActionNumber { get; set; }
        public string CustomerName { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string APQPItemId { get; set; } //partNumber        
        public string CAPAInitiatedBy { get; set; }
        public Nullable<int> DefectTypeId { get; set; }
        public Nullable<System.DateTime> OpenDateFrom { get; set; }
        public Nullable<System.DateTime> OpenDateTo { get; set; }        
    }
}
