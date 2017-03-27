using NPE.Core;
using NPE.Core.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES.Business.Repositories.Lookup
{
    public interface ILookupRepository
    {
        LookupCollections Query(List<Query> lookups);

        List<string> GetLookupNames();
    }
}
