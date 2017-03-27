using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.RFQ.Customer
{
    public class Contacts : CreateEditPropBase
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public short? PrefixId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Designation { get; set; }
        public string DirectPhone { get; set; }
        public string CellPhone { get; set; }
        public string Extension { get; set; }
        public string DirectFax { get; set; }
        public string Email { get; set; }
        public string Comments { get; set; }
        public bool IsDefault { get; set; }
        
        public MES.DTO.Library.Common.LookupFields PrefixItem { get; set; }
        public bool chkSelect { get; set; }
    }
}
