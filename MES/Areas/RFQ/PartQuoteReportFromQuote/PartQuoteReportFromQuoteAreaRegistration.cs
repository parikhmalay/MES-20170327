using System.Web.Mvc;

namespace MES.Areas.RFQ.PartQuoteReportFromQuote
{
    public class PartQuoteReportFromQuoteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "PartQuoteReportFromQuote";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PartQuoteReportFromQuote_default",
                "RFQ/PartQuoteReportFromQuote/{controller}/{action}/{id}",
                new { controllers = "PartQuoteReportFromQuote", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}