using System.Web.Mvc;

namespace MES.Areas.ShipmentTracking
{
    public class ShipmentTrackingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ShipmentTracking";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ShipmentTracking_default",
                "ShipmentTracking/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}