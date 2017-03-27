using System.Web.Mvc;

namespace MES.API.Areas.APQP.DocumentManagement
{
    public class DocumentManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DocumentManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DocumentManagement_Default",
                "APQP/DocumentManagement/{Controller}/{action}/{id}",
                new { controller = "DocumentManagement", action = "Index", id = UrlParameter.Optional });
        }
    }
}