using System.Web.Mvc;

namespace MES.API.Areas.APQP.DefectTracking
{
    public class DefectTrackingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DefectTracking";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DefectTracking_Default",
                "APQP/DefectTracking/{Controller}/{action}/{id}",
                new { controller = "DefectTracking", action = "Index", id = UrlParameter.Optional });
        }
    }
}