using System.Web.Mvc;

namespace MES.Areas.RFQ.DQ
{
    public class DQAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DQ";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DQ_default",
                "RFQ/DQ/{controller}/{action}/{id}",
                new { controllers = "SubmitQuote", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}