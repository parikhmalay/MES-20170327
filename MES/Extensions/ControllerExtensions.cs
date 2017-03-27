using Ninject;
using NPE.Business.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Extended
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Resolves the Repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="businessBase"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "controller")]
        public static T Resolve<T>(this Controller controller, T businessBase)
        {
            return Container.Resolve<T>(businessBase);
        }
        public static MES.DTO.Library.Identity.LoginUser GetCurrentUser()
        {
            MES.Business.Library.BO.UserManagement.UserManagement userObj = new Business.Library.BO.UserManagement.UserManagement();

            MES.DTO.Library.Identity.LoginUser userInfo = userObj.GetCurrentUserInfo().Result;
            return userInfo;

        }
    }
    public static class JsonHtmlExtensions
    {

        public static MvcHtmlString Json(this HtmlHelper helper,string type)
        {
            MES.Business.Library.BO.RoleManagement.Role roleObj = new Business.Library.BO.RoleManagement.Role();

            var model = roleObj.GetMenuList();
            return MvcHtmlString.Create(Newtonsoft.Json.JsonConvert.SerializeObject(model));

        }
    }

}