using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.Supplier
{
    public class SupplierAssessment : CreateEditPropBase
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string LeadAuditor { get; set; }
        public Nullable<DateTime> AuditDate { get; set; }
        public string Commodities { get; set; }
        public string PrimaryCustomer { get; set; }
        public string PrimaryIndustries { get; set; }
        public string ExportExperience { get; set; }
        public string LastYearSales { get; set; }
        public string PreviousYearSales { get; set; }
        public string NoOfShifts { get; set; }
        public string TotalMFGFloorSpace { get; set; }
        public string NoOfEmployees { get; set; }
        public decimal TotalScore { get; set; }
        public decimal WeightScore { get; set; }
        public string QualityScore { get; set; }
        public decimal EHSScore { get; set; }
        public decimal FinalEHSScore { get; set; }
        public string CoreCompetenciesList { get; set; }
        public string Strengths { get; set; }
        public string AreasforImprovement { get; set; }
        public bool IsDeleted { get; set; }
        public string Revision { get; set; }
        public string PreviousYearCompanyFinancials { get; set; }
        public string SupplimentaryEvidence{get;set;}
        public string EquipmentCapacityList_FileName { get; set; }
        public string EquipmentCapacityList_FilePath { get; set; }
        public bool IsNew { get; set; } 

        public List<AssessmentScopeOfWork> ScopeOfWork { get; set; }

        public List<AssessmentQuestionType> QuestionType { get; set; }
    }

    public class AssessmentScopeOfWork
    {
        public int Id { get; set; }
        public int AssessmentId { get; set; }
        public string ScopeOfWork { get; set; }
    }

    public class AssessmentQuestionType
    {
        public int Id { get; set; }
        public string QuestionType { get; set; }

        public List<AssessmentElement> AssessmentElement { get; set; }

    }

    public class AssessmentElement
    {
        public int Id { get; set; }
        public string ElementName { get; set; }
        public string ElementName_Chinese { get; set; }
        public int QuestionTypeId { get; set; }

        public List<AssessmentElementDetail> ElementDetails { get; set; }
    }

    public class AssessmentElementDetail
    {
        public int Id { get; set; }
        public int ElementId { get; set; }
        public string ElementDetail { get; set; }
        public string ElementDetail_Chinese { get; set; }
        public bool SubElementAvailable { get; set; }
        public int SortOrder { get; set; }
        public int Weight { get; set; }
        public bool IsOptionAvailable { get; set; }
        
        public AssessmentElementValue AssessmentElementValue { get; set; }

        public List<AssessmentSubElementDetail> SubElementDetail { get; set; }
    }

    public class AssessmentSubElementDetail
    {
        public int Id { get; set; }
        public int DetailId { get; set; }
        public string SubElementDetail { get; set; }
        public string SubElementDetail_Chinese { get; set; }
        public int SortOrder { get; set; }
        public string DataTypeField { get; set; }

        public AssessmentSubElementValue SubElementValue { get; set; }
    }

    public class AssessmentElementValue : CreateEditPropBase
    {
        public int AssementId { get; set; }
        public int DetailId { get; set; }
        //public Nullable<int> Weight { get; set; }
        public int Score { get; set; }
        public string SubTotal { get; set; }
        public string Observations { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string OptionValue { get; set; }
    }

    public class AssessmentSubElementValue : CreateEditPropBase
    {
        public int AssessmentId { get; set; }
        public int SubElementId { get; set; }
        public string SubElementValue { get; set; }

    }

    public class AssessmentListDetail
    {
        public int SupplierId { get; set; }
        public int AssessmentId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Revision { get; set; }
        public DateTime AuditDate { get; set; }
        public decimal TotalScore { get; set; }
        public string LeadAuditor { get; set; }
        public bool IsNew { get; set; }
    }
}
