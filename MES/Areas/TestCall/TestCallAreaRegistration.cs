using System.Web.Mvc;

namespace MES.API.Areas.TestCall
{
    public class TestCallAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TestCall";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TestCall_Default",
                "TestCall/{Controller}/{action}/{id}",
                new { controller = "Test", action = "Index", id = UrlParameter.Optional });
        }
    }
}