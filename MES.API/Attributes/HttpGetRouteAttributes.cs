using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace MES.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HttpGetRouteAttribute : Attribute, IActionHttpMethodProvider, IDirectRouteFactory, IHttpRouteInfoProvider
    {
        private static readonly Collection<HttpMethod> _supportedMethods = new Collection<HttpMethod>(new HttpMethod[]
		{
			HttpMethod.Get
		});

       public Collection<HttpMethod> HttpMethods
        {
            get
            {
                return HttpGetRouteAttribute._supportedMethods;
            }
        }

        public HttpGetRouteAttribute(string template)
		{
			if (template == null)
			{
				throw new ArgumentNullException("template");
			}
			this.Template = template;
		}
		RouteEntry IDirectRouteFactory.CreateRoute(DirectRouteFactoryContext context)
		{
			IDirectRouteBuilder directRouteBuilder = context.CreateBuilder(this.Template);
			directRouteBuilder.Name = this.Name;
			directRouteBuilder.Order = this.Order;
			return directRouteBuilder.Build();
		}

      
        public string Name
        {
            get;
            set;
        }
        public int Order
        {
            get;
            set;
        }
        public string Template
        {
            get;
            private set;
        }
    }
}