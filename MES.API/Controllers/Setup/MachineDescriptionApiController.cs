using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.MachineDescription;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("MachineDescriptionApi")]
    public class MachineDescriptionApiController : SecuredApiControllerBase
    {
        [Inject]
        public IMachineDescriptionRepository MachineDescriptionRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.MachineDescription.MachineDescription> Get(int Id)
        {
            var type = this.Resolve<IMachineDescriptionRepository>(MachineDescriptionRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the MachineDescription data.
        /// </summary>
        /// <param name="MachineDescription"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.MachineDescription.MachineDescription machineDescription)
        {
            var type = this.Resolve<IMachineDescriptionRepository>(MachineDescriptionRepository).Save(machineDescription);
            return type;
        }

        /// <summary>
        /// delete the machineDescription data.
        /// </summary>
        /// <param name="MachineDescriptionId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int machineDescriptionId)
        {
            var type = this.Resolve<IMachineDescriptionRepository>(MachineDescriptionRepository).Delete(machineDescriptionId);
            return type;
        }

        /// <summary>
        /// Get MachineDescription list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetMachineDescriptionList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.MachineDescription.MachineDescription>> GetMachineDescriptionList(GenericPage<MES.DTO.Library.Setup.MachineDescription.SearchCriteria> paging)
        {
            var type = this.Resolve<IMachineDescriptionRepository>(MachineDescriptionRepository).GetMachineDescriptionList(paging);
            return type;
        }
        #endregion Methods
    }
}