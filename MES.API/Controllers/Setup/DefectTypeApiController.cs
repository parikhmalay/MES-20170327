using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.DefectType;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("DefectTypeApi")]
    public class DefectTypeApiController : SecuredApiControllerBase
    {
        [Inject]
        public IDefectTypeRepository DefectTypeRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.DefectType.DefectType> Get(int Id)
        {
            var type = this.Resolve<IDefectTypeRepository>(DefectTypeRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the Defect Type data.
        /// </summary>
        /// <param name="DefectType"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.DefectType.DefectType defectType)
        {
            var type = this.Resolve<IDefectTypeRepository>(DefectTypeRepository).Save(defectType);
            return type;
        }

        /// <summary>
        /// delete the Defect Type data.
        /// </summary>
        /// <param name="DefectTypeId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int defectTypeId)
        {
            var type = this.Resolve<IDefectTypeRepository>(DefectTypeRepository).Delete(defectTypeId);
            return type;
        }

        /// <summary>
        /// Get DefectType list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetDefectTypeList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.DefectType.DefectType>> GetDefectTypeList(GenericPage<MES.DTO.Library.Setup.DefectType.SearchCriteria> paging)
        {
            var type = this.Resolve<IDefectTypeRepository>(DefectTypeRepository).GetDefectTypeList(paging);
            return type;
        }
        #endregion Methods
    }
}