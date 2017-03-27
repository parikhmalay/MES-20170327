using Ninject;
using NPE.Core.Extensions;
using NPE.Web.Common.AttributeBase;
using NPE.Web.Common.Controllers;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
//using MES.DTO.Library.Enumerator;

namespace MES.Controllers
{
    [SetAccessToken]
    public class WebControllerBase : DefaultWebControllerBase
    {
        [Inject]
        public override NPE.Core.IClientValidation WebValidator
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if User has logged in.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [current user not null]; otherwise, <c>false</c>.
        /// </value>
        public bool CurrentUserNotNull
        {
            get
            {
                return (CurrentUser != null && CurrentUser.GetSubjectId() != null);
            }
        }
    }
}