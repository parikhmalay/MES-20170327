using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Process;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("ProcessApi")]
    public class ProcessApiController : SecuredApiControllerBase
    {

        [Inject]
        public IProcessRepository ProcessRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// Process list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetProcesses")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Process.Process>> GetProcesses(GenericPage<string> page)
        {
            var type = this.Resolve<IProcessRepository>(ProcessRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get Process list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetProcessList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Process.Process>> GetProcessList(GenericPage<MES.DTO.Library.Setup.Process.SearchCriteria> paging)
        {
            var type = this.Resolve<IProcessRepository>(ProcessRepository).GetProcesses(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.Process.Process> Get(int Id)
        {
            var type = this.Resolve<IProcessRepository>(ProcessRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the process data.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Process.Process process)
        {
            var type = this.Resolve<IProcessRepository>(ProcessRepository).Save(process);
            return type;
        }
        /// <summary>
        /// delete the process data.
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int processId)
        {
            var type = this.Resolve<IProcessRepository>(ProcessRepository).Delete(processId);
            return type;
        }

        #endregion
    }
}
