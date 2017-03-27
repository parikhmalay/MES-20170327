using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Commodity;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("CommodityApi")]
    public class CommodityApiController : SecuredApiControllerBase
    {
        [Inject]
        public ICommodityRepository CommodityRepository { get; set; }

        #region Methods

        /// <summary>
        /// save the Commodity data.
        /// </summary>
        /// <param name="commodity"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Commodity.Commodity commodity)
        {
            var type = this.Resolve<ICommodityRepository>(CommodityRepository).Save(commodity);
            return type;
        }

        /// <summary>
        /// delete the Commodity data.
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int commodityId)
        {
            var type = this.Resolve<ICommodityRepository>(CommodityRepository).Delete(commodityId);
            return type;
        }
       
        /// <summary>
        /// Get Commodity list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetCommodityList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Commodity.Commodity>> GetCommodityList(GenericPage<MES.DTO.Library.Setup.Commodity.SearchCriteria> paging)
        {
            var type = this.Resolve<ICommodityRepository>(CommodityRepository).GetCommodityList(paging);
            return type;
        }

        #endregion Methods
    }
}