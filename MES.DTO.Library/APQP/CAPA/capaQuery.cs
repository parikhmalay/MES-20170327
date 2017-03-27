using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MES.DTO.Library.Common;

namespace MES.DTO.Library.APQP.CAPA
{
    public class capaQuery : CreateEditPropBase
    {
        public short Id { get; set; }
        public string Value { get; set; }
    }
}