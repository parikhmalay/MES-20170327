using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.IndustryType;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("IndustryTypeApi")]
    public class IndustryTypeApiController : SecuredApiControllerBase
    {

        [Inject]
        public IIndustryTypeRepository IndustryTypeRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// IndustryType list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetIndustryTypes")]
        public ITypedResponse<List<MES.DTO.Library.Setup.IndustryType.IndustryType>> GetIndustryTypes(GenericPage<string> page)
        {
            var type = this.Resolve<IIndustryTypeRepository>(IndustryTypeRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get IndustryTypes list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetIndustryTypeList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.IndustryType.IndustryType>> GetIndustryTypeList(GenericPage<MES.DTO.Library.Setup.IndustryType.SearchCriteria> paging)
        {
            var type = this.Resolve<IIndustryTypeRepository>(IndustryTypeRepository).GetIndustryTypes(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.IndustryType.IndustryType> Get(int Id)
        {
            var type = this.Resolve<IIndustryTypeRepository>(IndustryTypeRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the IndustryType data.
        /// </summary>
        /// <param name="IndustryType"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.IndustryType.IndustryType IndustryType)
        {
            var type = this.Resolve<IIndustryTypeRepository>(IndustryTypeRepository).Save(IndustryType);
            return type;
        }
        /// <summary>
        /// delete the IndustryType data.
        /// </summary>
        /// <param name="IndustryTypeId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int IndustryTypeId)
        {
            var type = this.Resolve<IIndustryTypeRepository>(IndustryTypeRepository).Delete(IndustryTypeId);
            return type;
        }

        #endregion
    }
}
