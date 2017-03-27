using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.MachiningDescription;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("MachiningDescriptionApi")]
    public class MachiningDescriptionApiController : SecuredApiControllerBase
    {
        [Inject]
        public IMachiningDescriptionRepository MachiningDescriptionRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.MachiningDescription.MachiningDescription> Get(int Id)
        {
            var type = this.Resolve<IMachiningDescriptionRepository>(MachiningDescriptionRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the MachiningDescription data.
        /// </summary>
        /// <param name="MachiningDescription"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.MachiningDescription.MachiningDescription machiningDescription)
        {
            var type = this.Resolve<IMachiningDescriptionRepository>(MachiningDescriptionRepository).Save(machiningDescription);
            return type;
        }

        /// <summary>
        /// delete the machiningDescription data.
        /// </summary>
        /// <param name="MachiningDescriptionId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int machiningDescriptionId)
        {
            var type = this.Resolve<IMachiningDescriptionRepository>(MachiningDescriptionRepository).Delete(machiningDescriptionId);
            return type;
        }

        /// <summary>
        /// Get MachiningDescription list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetMachiningDescriptionList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.MachiningDescription.MachiningDescription>> GetMachiningDescriptionList(GenericPage<MES.DTO.Library.Setup.MachiningDescription.SearchCriteria> paging)
        {
            var type = this.Resolve<IMachiningDescriptionRepository>(MachiningDescriptionRepository).GetMachiningDescriptionList(paging);
            return type;
        }
        #endregion Methods
    }
}