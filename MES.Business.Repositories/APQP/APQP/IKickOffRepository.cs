﻿using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.APQP
{
    public interface IKickOffRepository : ICrudMethods<MES.DTO.Library.APQP.APQP.KickOff, int?, string,
          MES.DTO.Library.APQP.APQP.KickOff, int, bool?, int, MES.DTO.Library.APQP.APQP.KickOff>
    {
        ITypedResponse<List<MES.DTO.Library.APQP.APQP.KickOff>> GetKickOffList(NPE.Core.IPage<MES.DTO.Library.APQP.APQP.SearchCriteria> searchCriteria);
    }
}