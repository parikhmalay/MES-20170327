using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.NonAwardFeedback;
using NPE.Core;
using NPE.Core.Extended;
namespace MES.API.Controllers.Setup
{
    [AdminPrefix("NonAwardFeedbackApi")]
    public class NonAwardFeedbackApiController : SecuredApiControllerBase
    {

        [Inject]
        public INonAwardFeedbackRepository NonAwardFeedbackRepository { get; set; }

        #region Methods


        /// <summary>
        /// Get
        /// 
        /// NonAwardFeedback list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetNonAwardFeedbacks")]
        public ITypedResponse<List<MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>> GetNonAwardFeedbacks(GenericPage<string> page)
        {
            var type = this.Resolve<INonAwardFeedbackRepository>(NonAwardFeedbackRepository).Search(page);
            return type;
        }

        /// <summary>
        /// Get NonAwardFeedbacks list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetNonAwardFeedbackList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback>> GetNonAwardFeedbackList(GenericPage<MES.DTO.Library.Setup.NonAwardFeedback.SearchCriteria> paging)
        {
            var type = this.Resolve<INonAwardFeedbackRepository>(NonAwardFeedbackRepository).GetNonAwardFeedbacks(paging);
            return type;
        }


        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback> Get(int Id)
        {
            var type = this.Resolve<INonAwardFeedbackRepository>(NonAwardFeedbackRepository).FindById(Id);
            return type;
        }


        /// <summary>
        /// save the nonAwardFeedback data.
        /// </summary>
        /// <param name="nonAwardFeedback"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.NonAwardFeedback.NonAwardFeedback nonAwardFeedback)
        {
            var type = this.Resolve<INonAwardFeedbackRepository>(NonAwardFeedbackRepository).Save(nonAwardFeedback);
            return type;
        }
        /// <summary>
        /// delete the nonAwardFeedback data.
        /// </summary>
        /// <param name="nonAwardFeedbackId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int nonAwardFeedbackId)
        {
            var type = this.Resolve<INonAwardFeedbackRepository>(NonAwardFeedbackRepository).Delete(nonAwardFeedbackId);
            return type;
        }

        #endregion
    }
}
