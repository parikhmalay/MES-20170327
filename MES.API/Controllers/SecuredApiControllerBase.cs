using MES.API.Attributes;
using MES.DTO.Library.Base;
using Ninject;
using NPE.Business.Common;
using NPE.Core.Identity;
using NPE.Web.Common.AttributeBase;
using NPE.Web.Common.Controllers;
using NPE.Web.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;
namespace MES.API.Controllers
{
    [CustomAuthorizeAttribute]
    public class SecuredApiControllerBase : DefaultApiControllerBase
    {
        [Inject]
        public override NPE.Core.IClientValidation ApiValidator
        {
            get;
            set;
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        protected UpdateOptions GetUpdateOptions(bool createOrUpdate)
        {
            return createOrUpdate ? UpdateOptions.CreateOrUpdate : UpdateOptions.CreateOnly;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("NPE.Web.Restrictions", "NPE0002:HttpGetPostAttributeFound"), ApiExplorerSettings(IgnoreApi = true)]
        public override bool ValidateClient(string apiKey)
        {
            var validator = Container.Resolve<NPE.Core.IClientValidation>(ApiValidator);
            if (validator != null)
            {
                _APIClientInfo = validator.GetClientInfo(apiKey);
                string culture = string.Empty;

                if (_APIClientInfo != null)
                {
                    CultureHelper.SetCulture(_APIClientInfo, User);
                }
                else
                    _APIClientInfo = new ClientAPIInfo();
            }
            return (_APIClientInfo.Id > 0);
        }

    }
}