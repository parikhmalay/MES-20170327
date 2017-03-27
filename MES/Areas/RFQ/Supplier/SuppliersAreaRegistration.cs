using System.Web.Mvc;

namespace MES.API.Areas.RFQ.Supplier
{
    public class SuppliersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Suppliers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Suppliers_Default",
                "RFQ/Supplier/{Controller}/{action}/{id}",
                new { controller = "Suppliers", action = "Index", id = UrlParameter.Optional });
        }
    }
}