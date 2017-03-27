using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MES.Areas.RFQ.Quote
{
    public class QuoteAreaRegistration : AreaRegistration
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
                "Quote_default",
                "RFQ/Quote/{controller}/{action}/{id}",
                new { controllers = "Quote", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}