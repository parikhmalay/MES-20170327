using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Remarks;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("RemarksApi")]
    public class RemarksApiController : SecuredApiControllerBase
    {

        [Inject]
        public IRemarksRepository RemarksRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// Remarks list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRemarks")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Remarks.Remarks>> GetRemarkss(GenericPage<string> page)
        {
            var type = this.Resolve<IRemarksRepository>(RemarksRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get Remarks list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetRemarksList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Remarks.Remarks>> GetRemarksList(GenericPage<MES.DTO.Library.Setup.Remarks.SearchCriteria> paging)
        {
            var type = this.Resolve<IRemarksRepository>(RemarksRepository).GetRemarks(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.Remarks.Remarks> Get(int Id)
        {
            var type = this.Resolve<IRemarksRepository>(RemarksRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the remarks data.
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Remarks.Remarks remarks)
        {
            var type = this.Resolve<IRemarksRepository>(RemarksRepository).Save(remarks);
            return type;
        }
        /// <summary>
        /// delete the remarks data.
        /// </summary>
        /// <param name="remarksId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int remarksId)
        {
            var type = this.Resolve<IRemarksRepository>(RemarksRepository).Delete(remarksId);
            return type;
        }

        #endregion
    }
}
