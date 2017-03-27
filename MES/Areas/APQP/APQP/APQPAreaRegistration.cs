using System.Web.Mvc;

namespace MES.API.Areas.APQP.APQP
{
    public class APQPAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "APQP";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "APQP_Default",
                "APQP/APQP/{Controller}/{action}/{id}",
                new { controller = "APQP", action = "Index", id = UrlParameter.Optional });
        }
    }
}