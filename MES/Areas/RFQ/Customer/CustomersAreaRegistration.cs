using System.Web.Mvc;

namespace MES.API.Areas.RFQ.Customer
{
    public class CustomersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Customers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Customers_Default",
                "RFQ/Customer/{Controller}/{action}/{id}",
                new { controller = "Customers", action = "Index", id = UrlParameter.Optional });
        }
    }
}