using System.Web.Mvc;

namespace MES.Areas.RFQ.SupplierQuote
{
    public class SupplierQuoteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SupplierQuote";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SupplierQuote_default",
                "RFQ/SQ/{controller}/{action}/{id}",
                new { controllers = "SupplierQuote", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}