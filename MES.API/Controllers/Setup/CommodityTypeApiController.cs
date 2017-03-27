using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.CommodityType;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("CommodityTypeApi")]
    public class CommodityTypeApiController : SecuredApiControllerBase
    {
        [Inject]
        public ICommodityTypeRepository CommodityTypeRepository { get; set; }

        #region Methods

        /// <summary>
        /// save the CommodityType data.
        /// </summary>
        /// <param name="commodityType"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.CommodityType.CommodityType commodityType)
        {
            var type = this.Resolve<ICommodityTypeRepository>(CommodityTypeRepository).Save(commodityType);
            return type;
        }

        /// <summary>
        /// delete the CommodityType data.
        /// </summary>
        /// <param name="commodityTypeId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int commodityTypeId)
        {
            var type = this.Resolve<ICommodityTypeRepository>(CommodityTypeRepository).Delete(commodityTypeId);
            return type;
        }
       
        /// <summary>
        /// Get CommodityType list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetCommodityTypesList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.CommodityType.CommodityType>> GetCommodityTypesList(GenericPage<MES.DTO.Library.Setup.CommodityType.SearchCriteria> paging)
        {
            var type = this.Resolve<ICommodityTypeRepository>(CommodityTypeRepository).GetCommodityTypesList(paging);
            return type;
        }

        #endregion Methods
    }
}