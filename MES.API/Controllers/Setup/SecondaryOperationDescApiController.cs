using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.SecondaryOperationDesc;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("SecondaryOperationDescApi")]
    public class SecondaryOperationDescApiController : SecuredApiControllerBase
    {

        [Inject]
        public ISecondaryOperationDescRepository SecondaryOperationDescRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// SecondaryOperationDesc list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetSecondaryOperationDescs")]
        public ITypedResponse<List<MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>> GetSecondaryOperationDescs(GenericPage<string> page)
        {
            var type = this.Resolve<ISecondaryOperationDescRepository>(SecondaryOperationDescRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get SecondaryOperationDescs list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetSecondaryOperationDescList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc>> GetSecondaryOperationDescList(GenericPage<MES.DTO.Library.Setup.SecondaryOperationDesc.SearchCriteria> paging)
        {
            var type = this.Resolve<ISecondaryOperationDescRepository>(SecondaryOperationDescRepository).GetSecondaryOperationDescs(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc> Get(int Id)
        {
            var type = this.Resolve<ISecondaryOperationDescRepository>(SecondaryOperationDescRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the secondaryOperationDesc data.
        /// </summary>
        /// <param name="secondaryOperationDesc"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.SecondaryOperationDesc.SecondaryOperationDesc secondaryOperationDesc)
        {
            var type = this.Resolve<ISecondaryOperationDescRepository>(SecondaryOperationDescRepository).Save(secondaryOperationDesc);
            return type;
        }
        /// <summary>
        /// delete the secondaryOperationDesc data.
        /// </summary>
        /// <param name="secondaryOperationDescId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int secondaryOperationDescId)
        {
            var type = this.Resolve<ISecondaryOperationDescRepository>(SecondaryOperationDescRepository).Delete(secondaryOperationDescId);
            return type;
        }

        #endregion
    }
}
