using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.CAPA
{
    public interface ICAPAPartAffectedDetailRepository : ICrudMethods<MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail, int?, string,
          MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail, int, bool?, int, MES.DTO.Library.APQP.CAPA.capaPartAffectedDetail>
    {
    }
}
