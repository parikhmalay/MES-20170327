using NPE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.APQP.CAPA
{
    public interface IPartDocumentRepository : ICrudMethods<MES.DTO.Library.APQP.CAPA.capaPartDocument, int?, string,
          MES.DTO.Library.APQP.CAPA.capaPartDocument, int, bool?, int, MES.DTO.Library.APQP.CAPA.capaPartDocument>
    {
        List<MES.DTO.Library.APQP.CAPA.capaPartDocument> GetPartDocumentList(int capaAffectedPartDetailId, int AssociatedToId);
    }
}
