using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.RFQ.Supplier
{
    public class Suppliers : CreateEditPropBase
    {
        public int Id { get; set; }
        public string SupplierCode { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<short> CountryId { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Website { get; set; }
        public string CompanyPhone1 { get; set; }
        public string CompanyPhone2 { get; set; }
        public string CompanyFax { get; set; }
        public string Comments { get; set; }
        public short WorkQualityRating { get; set; }
        public short TimelineRating { get; set; }
        public short CostingRating { get; set; }
        public Nullable<Guid> SQId { get; set; }
        public string SupplierQuality { get; set; }
        public Nullable<short> Status { get; set; }
        public Nullable<bool> IsCurrentSupplier { get; set; }
        public string IsCurrentSupplierText { get; set; }
        public Nullable<System.DateTime> AssessmentDate { get; set; }
        public string Score { get; set; }
        public string SQCertificationFilePath { get; set; }
        public string AssessmentScore { get; set; }
        public string Email { get; set; }

        public string SCEmail { get; set; }
        public string SCName { get; set; }
        public string SCPhone { get; set; }
        public string SQEmail { get; set; }
        public string SQName { get; set; }

        public virtual List<MES.DTO.Library.Common.LookupFields> CommoditySuppliers { get; set; }
        public virtual List<Contacts> lstContact { get; set; }
        public virtual List<Documents> lstDocument { get; set; }
        public virtual List<AssessmentListDetail> lstAssessment { get; set; }
        public bool chkSelect { get; set; }
    }

    public class SearchCriteria
    {
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Website { get; set; }
        public string CompanyPhone1 { get; set; }
        public Nullable<short> Status { get; set; }
        public string CommodityIds { get; set; }
        public short? WorkQualityRating { get; set; }
        public short? TimelineRating { get; set; }
        public short? CostingRating { get; set; }
        public bool? IsCurrentSupplier { get; set; }
    }
}
