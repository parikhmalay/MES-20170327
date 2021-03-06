//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MES.Data.Library
{
    using System;
    using System.Collections.Generic;
    
    public partial class SupplierAssessment
    {
        public SupplierAssessment()
        {
            this.AssessmentElementValues = new HashSet<AssessmentElementValue>();
            this.AssessmentScopeOfWorks = new HashSet<AssessmentScopeOfWork>();
            this.AssessmentSubElementValues = new HashSet<AssessmentSubElementValue>();
        }
    
        public int Id { get; set; }
        public string Revision { get; set; }
        public Nullable<int> ParentAssessmentId { get; set; }
        public int SupplierId { get; set; }
        public string LeadAuditor { get; set; }
        public System.DateTime AuditDate { get; set; }
        public string Commodities { get; set; }
        public string PrimaryCustomer { get; set; }
        public string PrimaryIndustries { get; set; }
        public string ExportExperience { get; set; }
        public string LastYearSales { get; set; }
        public string PreviousYearSales { get; set; }
        public string NoOfShifts { get; set; }
        public string TotalMFGFloorSpace { get; set; }
        public string NoOfEmployees { get; set; }
        public Nullable<decimal> TotalScore { get; set; }
        public string QualityScore { get; set; }
        public string CoreCompetenciesList { get; set; }
        public string Strengths { get; set; }
        public string AreasforImprovement { get; set; }
        public bool IsDeleted { get; set; }
        public string PreviousYearCompanyFinanacials { get; set; }
        public string SupplimentaryEvidence { get; set; }
        public string EquipmentCapacityList_FileName { get; set; }
        public string EquipmentCapacityList_FilePath { get; set; }
        public Nullable<decimal> EHSScore { get; set; }
        public Nullable<decimal> FinalEHSScore { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsNew { get; set; }
    
        public virtual ICollection<AssessmentElementValue> AssessmentElementValues { get; set; }
        public virtual ICollection<AssessmentScopeOfWork> AssessmentScopeOfWorks { get; set; }
        public virtual ICollection<AssessmentSubElementValue> AssessmentSubElementValues { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
