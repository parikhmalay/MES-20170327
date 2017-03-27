using System.Web.Mvc;

namespace MES.API.Areas.APQP.ChangeRequest
{
    public class ChangeRequestAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ChangeRequest";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ChangeRequest_Default",
                "APQP/ChangeRequest/{Controller}/{action}/{id}",
                new { controller = "ChangeRequest", action = "Index", id = UrlParameter.Optional });
        }
    }
}