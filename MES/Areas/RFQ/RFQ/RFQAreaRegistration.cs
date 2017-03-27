using System.Web.Mvc;

namespace MES.Areas.RFQ.RFQ
{
    public class RFQAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RFQ";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "RFQ_default",
                "RFQ/RFQ/{controller}/{action}/{id}",
                new { controllers = "RFQ", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}