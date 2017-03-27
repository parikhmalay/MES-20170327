using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.CoatingType;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("CoatingTypeApi")]
    public class CoatingTypeApiController : SecuredApiControllerBase
    {
        [Inject]
        public ICoatingTypeRepository CoatingTypeRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.CoatingType.CoatingType> Get(int Id)
        {
            var type = this.Resolve<ICoatingTypeRepository>(CoatingTypeRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Coating Type data.
        /// </summary>
        /// <param name="CoatingType"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.CoatingType.CoatingType coatingType)
        {
            var type = this.Resolve<ICoatingTypeRepository>(CoatingTypeRepository).Save(coatingType);
            return type;
        }

        /// <summary>
        /// delete the Coating Type data.
        /// </summary>
        /// <param name="CoatingTypeId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int coatingTypeId)
        {
            var type = this.Resolve<ICoatingTypeRepository>(CoatingTypeRepository).Delete(coatingTypeId);
            return type;
        }

        /// <summary>
        /// Get CoatingType list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetCoatingTypeList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.CoatingType.CoatingType>> GetCoatingTypeList(GenericPage<MES.DTO.Library.Setup.CoatingType.SearchCriteria> paging)
        {
            var type = this.Resolve<ICoatingTypeRepository>(CoatingTypeRepository).GetCoatingTypeList(paging);
            return type;
        }
        #endregion Methods
    }
}