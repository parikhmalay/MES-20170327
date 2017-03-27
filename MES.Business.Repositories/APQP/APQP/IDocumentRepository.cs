using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.APQP
{
    public interface IDocumentRepository : ICrudMethods<MES.DTO.Library.APQP.APQP.Document, int?, string,
          MES.DTO.Library.APQP.APQP.Document, int, bool?, int, MES.DTO.Library.APQP.APQP.Document>
    {

    }
}
