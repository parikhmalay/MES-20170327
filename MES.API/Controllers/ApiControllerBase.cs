using Ninject;
using NPE.Web.Common.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;
using MES.API.Extensions;
using NPE.Business.Common;

namespace MES.API
{
    public class ApiControllerBase : DefaultApiControllerBase
    {
        [Inject]
        public override NPE.Core.IClientValidation ApiValidator
        {
            get;
            set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("NPE.Web.Restrictions", "NPE0002:HttpGetPostAttributeFound"), ApiExplorerSettings(IgnoreApi = true)]
        public override bool ValidateClient(string apiKey)
        {
            throw new NotImplementedException();
        }
    }
}