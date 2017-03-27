using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.RFQ
{
    public class RFQSuppliers : CreateEditPropBase
    {
        public int Id { get; set; }
        public string RFQId { get; set; }
        public int SupplierId { get; set; }
        public string[] SupplierIdList { get; set; }       
        public string[] RFQSupplierIdList { get; set; }
        public string UniqueURL { get; set; }
        public Nullable<System.DateTime> QuoteExpireDate { get; set; }
        public Nullable<System.DateTime> QuoteDate { get; set; }
        public bool IsURLActive { get; set; }
        public string Remarks { get; set; }
        public string OriginalURL { get; set; }
        public Nullable<bool> NoQuote { get; set; }
        public Nullable<bool> IsQuoteTypeDQ { get; set; }
        public string QuoteTypeDQ { get; set; }

        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string CompanyPhone1 { get; set; }
    }
    public class RfqSupplierSearchCriteria
    {
        public string RFQId { get; set; }
        public string Supplier { get; set; }
        public List<MES.DTO.Library.Common.LookupFields> CountryIds { get; set; }
        public List<MES.DTO.Library.Common.LookupFields> CommodityItemIds { get; set; }
    }
}
