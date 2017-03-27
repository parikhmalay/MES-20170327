using System.Web.Mvc;

namespace MES.API.Areas.RFQ.Reports
{
    public class ReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Reports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Reports_Default",
                "RFQ/Reports/{Controller}/{action}/{id}",
                new { controller = "Reports", action = "Index", id = UrlParameter.Optional });
        }
    }
}