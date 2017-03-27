using Ninject;
using MES.Business.Repositories.Lookup;
using NPE.Core;
using NPE.Core.Extended;
using NPE.Core.Helpers;
using System.Collections.Generic;
using MES.API.Attributes;
using MES.API.Extensions;
using System.Web.Http;

namespace MES.API.Controllers
{
    [AdminPrefix("Lookup")]
    public class LookupController : ApiController
    {
        [Inject]
        public ILookupRepository LookupRepository { get; set; }

      /// <summary>
        /// Queries the specified lookups.
        /// </summary>
        /// <param name="lookups">The lookups.</param>
        /// <returns></returns>
        [HttpPostRoute("Query")]
        public LookupCollections Query(List<Query> lookups)
        {
            return this.Resolve<ILookupRepository>(LookupRepository).Query(lookups);
        }

        /// <summary>
        /// Gets the names of lookups.
        /// </summary>
        /// <returns></returns>
        [HttpGetRoute("Names")]
        public List<string> GetNames()
        {
            return this.Resolve<ILookupRepository>(LookupRepository).GetLookupNames();
        }
    }
}