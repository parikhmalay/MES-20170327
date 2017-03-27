using IdentityModel.Client;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using MES.Authentication;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace MES
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            #region IDServer config
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Trace().CreateLogger();

            var idSvrFactory = Factory.Configure(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "MES",
                    RequireSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["RequireSSL"]) ? true : false,
                    SigningCertificate = LoadCertificate(),
                    //CorsPolicy = CorsPolicy.AllowAll,
                    Factory = idSvrFactory,

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions()
                    {
                        LoginPageLinks = new List<LoginPageLink>() { 
                            new LoginPageLink() {
                                   Text = "Forgot Username/Password?", 
                                   Href = ConfigurationManager.AppSettings["RecoverAccountUrl"]
                            }
                        },
                        //IdentityProviders = ConfigureIdentityProviders,
                        EnableSignOutPrompt = false,
                        EnablePostSignOutAutoRedirect = true
                    }

                });
            });
            #endregion

            #region Client Auth Config
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["Authority"],
                ClientId = "MES",
                Scope = "openid profile roles Api",

                ResponseType = "id_token token",

                SignInAsAuthenticationType = "Cookies",

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = n =>
                    {
                        if (n.Exception.Message.StartsWith("IDX10316") ||
                            n.Exception.Message.Contains("IDX10311") ||
                            n.Exception.Message.Contains("IDX10301"))
                        {
                            n.HandleResponse();
                            n.Response.Redirect("/");
                        }
                        return System.Threading.Tasks.Task.FromResult<object>(null);
                    },
                    SecurityTokenValidated = async n =>
                    {
                        var nid = new ClaimsIdentity(
                            n.AuthenticationTicket.Identity.AuthenticationType,
                            IdentityServer3.Core.Constants.ClaimTypes.GivenName,
                            IdentityServer3.Core.Constants.ClaimTypes.Role);

                        // get userinfo data
                        var userInfoClient = new UserInfoClient(
                            new Uri(n.Options.Authority + "/connect/userinfo"),
                            n.ProtocolMessage.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();
                        userInfo.Claims.ToList().ForEach(ui => nid.AddClaim(new Claim(ui.Item1, ui.Item2)));

                        // keep the id_token for logout
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        // add access token for sample API
                        nid.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                        // keep track of access token expiration
                        nid.AddClaim(new Claim("expires_at", DateTimeOffset.Now.AddSeconds(int.Parse(n.ProtocolMessage.ExpiresIn)).ToString()));

                        // add some other app specific claim
                        //nid.AddClaim(new Claim("app_specific", "some data"));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = async n =>
                    {
                        var uri = n.Request.Uri.GetLeftPart(UriPartial.Authority);
                        n.ProtocolMessage.RedirectUri = String.Format("{0}", uri);
                        n.ProtocolMessage.PostLogoutRedirectUri = uri;
                        if (n.ProtocolMessage.RequestType == Microsoft.IdentityModel.Protocols.OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }
                    }
                }
            });
            #endregion

            #region Load Maps
            NPE.Core.MapLoader.LoadMaps(ConfigurationManager.AppSettings["Business.Maps"] ?? System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"bin\MES.Business.Mapping.dll");
            NPE.Core.MapLoader.LoadMaps(ConfigurationManager.AppSettings["ModelBuilder.Maps"] ?? System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"bin\MES.ModelBuilder.Library.dll");
            #endregion
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\Authentication\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}