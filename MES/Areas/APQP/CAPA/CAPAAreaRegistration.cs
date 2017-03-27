using System.Web.Mvc;

namespace MES.Areas.APQP.CAPA
{
    public class CAPAAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CAPA";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CAPA_default",
                "APQP/CAPA/{controller}/{action}/{id}",
                new { controller = "CAPA", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}