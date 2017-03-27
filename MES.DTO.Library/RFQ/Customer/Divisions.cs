using MES.DTO.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.DTO.Library.RFQ.Customer
{
    public class Divisions : CreateEditPropBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<short> CountryId { get; set; }
        public string Zip { get; set; }
        public string Website { get; set; }
        public string CompanyPhone1 { get; set; }
        public string CompanyPhone2 { get; set; }
        public string CompanyFax { get; set; }
        public string Comments { get; set; }
        public Nullable<short> PaymentRating { get; set; }
        public Nullable<Guid> SAMId { get; set; }

        public MES.DTO.Library.Common.LookupFields SAM { get; set; }
        public bool chkSelect { get; set; }
    }
}
