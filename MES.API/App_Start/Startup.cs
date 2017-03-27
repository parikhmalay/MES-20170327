using IdentityServer3.AccessTokenValidation;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Web;

namespace MES.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //stop incoming claims to be mapped into ASP.NET claims
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["Authority"],
                RequiredScopes = new[] { "Api" }
            });

        }
    }
}