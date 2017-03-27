using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.RFQ.Customer
{
    public class Address : CreateEditPropBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AddressTypeId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Nullable<short> CountryId { get; set; }
        public string Zip { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDefault { get; set; }

        public MES.DTO.Library.Common.LookupFields AddressTypeItem { get; set; }
        public bool chkSelect { get; set; }
    }
}
