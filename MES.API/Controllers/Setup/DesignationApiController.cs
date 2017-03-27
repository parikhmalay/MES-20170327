using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.Designation;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("DesignationApi")]
    public class DesignationApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDesignationRepository DesignationRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.Designation.Designation> Get(int Id)
        {
            var type = this.Resolve<IDesignationRepository>(DesignationRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Designation data.
        /// </summary>
        /// <param name="Designation"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.Designation.Designation designation)
        {
            var type = this.Resolve<IDesignationRepository>(DesignationRepository).Save(designation);
            return type;
        }

        /// <summary>
        /// delete the Designation data.
        /// </summary>
        /// <param name="DesignationId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int designationId)
        {
            var type = this.Resolve<IDesignationRepository>(DesignationRepository).Delete(designationId);
            return type;
        }

        /// <summary>
        /// Get Designation list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDesignationList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.Designation.Designation>> GetDesignationList(GenericPage<MES.DTO.Library.Setup.Designation.SearchCriteria> paging)
        {
            var type = this.Resolve<IDesignationRepository>(DesignationRepository).GetDesignationList(paging);
            return type;
        }
        #endregion Methods
    }
}