using MES.API.Attributes;
using MES.API.Extensions;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MES.Business.Repositories.Setup.EmailTemplate;
using NPE.Core;
using NPE.Core.Extended;

namespace MES.API.Controllers.Setup
{
    [AdminPrefix("EmailTemplateApi")]
    public class EmailTemplateApiController : SecuredApiControllerBase
    {
        [Inject]
        public IEmailTemplateRepository EmailTemplateRepository { get; set; }

        #region Methods

        /// <summary>
        /// Get data.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGetRoute("Get")]
        public ITypedResponse<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate> Get(int Id)
        {
            var type = this.Resolve<IEmailTemplateRepository>(EmailTemplateRepository).FindById(Id);
            return type;
        }

        /// <summary>
        /// save the EmailTemplate data.
        /// </summary>
        /// <param name="EmailTemplate"></param>
        /// <returns></returns>
        [HttpPostRoute("Save")]
        public ITypedResponse<int?> Save(MES.DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate)
        {
            var type = this.Resolve<IEmailTemplateRepository>(EmailTemplateRepository).Save(emailTemplate);
            return type;
        }

        /// <summary>
        /// delete the EmailTemplate data.
        /// </summary>
        /// <param name="EmailTemplateId"></param>
        /// <returns></returns>
        [HttpPostRoute("Delete")]
        public ITypedResponse<bool?> Delete(int emailTemplateId)
        {
            var type = this.Resolve<IEmailTemplateRepository>(EmailTemplateRepository).Delete(emailTemplateId);
            return type;
        }

        /// <summary>
        /// Get EmailTemplate list.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpPostRoute("GetEmailTemplateList")]
        public ITypedResponse<List<MES.DTO.Library.Setup.EmailTemplate.EmailTemplate>> GetEmailTemplateList(GenericPage<MES.DTO.Library.Setup.EmailTemplate.SearchCriteria> paging)
        {
            var type = this.Resolve<IEmailTemplateRepository>(EmailTemplateRepository).GetEmailTemplateList(paging);
            return type;
        }

        /// <summary>
        /// send a test email.
        /// </summary>
        /// <param name="EmailTemplate"></param>
        /// <returns></returns>
        [HttpPostRoute("SendEmail")]
        public ITypedResponse<bool?> SendEmail(MES.DTO.Library.Setup.EmailTemplate.EmailTemplate emailTemplate)
        {
            var type = this.Resolve<IEmailTemplateRepository>(EmailTemplateRepository).SendEmail(emailTemplate);
            return type;
        }
        #endregion Methods
    }
}