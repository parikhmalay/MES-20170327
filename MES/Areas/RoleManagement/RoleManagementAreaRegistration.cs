using System.Web.Mvc;

namespace MES.Areas.RoleManagement
{
    public class RoleManagementAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "RoleManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "RoleManagement_default",
                "RoleManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}