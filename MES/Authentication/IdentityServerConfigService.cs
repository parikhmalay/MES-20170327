//using Account.IdentityServer.Data.Library;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Thinktecture.IdentityServer.Core.Configuration;
//using Thinktecture.IdentityServer.Core.Services;

//namespace MES.Authentication
//{
//    public static class IdentityServerServiceFactoryExtensions
//    {
//        //https://github.com/IdentityServer/IdentityServer3.EntityFramework/blob/master/Source/Core.EntityFramework/Extensions/IdentityServerServiceFactoryExtensions.cs
//        public static void IdentitySreverEFConfiguration(this IdentityServerServiceFactory factory, Thinktecture.IdentityServer.EntityFramework.EntityFrameworkServiceOptions option)
//        {
//            factory.Register(new Registration<ClientConfigurationDbContext>(resolver => new ClientConfigurationDbContext(option.ConnectionString, option.Schema)));
//            factory.Register(new Registration<OperationalDbContext>(resolver => new OperationalDbContext(option.ConnectionString, option.Schema)));
//            factory.Register(new Registration<ScopeConfigurationDbContext>(resolver => new ScopeConfigurationDbContext(option.ConnectionString, option.Schema)));

//            factory.ScopeStore = new Registration<IScopeStore, ScopeStore>();
//            factory.ClientStore = new Registration<IClientStore, ClientStore>();

//            /*factory.AuthorizationCodeStore = new Registration<IAuthorizationCodeStore, AuthorizationCodeStore>();
//            factory.TokenHandleStore = new Registration<ITokenHandleStore, TokenHandleStore>();
//            factory.ConsentStore = new Registration<IConsentStore, ConsentStore>();
//            factory.RefreshTokenStore = new Registration<IRefreshTokenStore, RefreshTokenStore>();
//            factory.CorsPolicyService = new Thinktecture.IdentityServer.EntityFramework.ClientConfigurationCorsPolicyRegistration(option);
//            var tokenCleanup = new Thinktecture.IdentityServer.EntityFramework.TokenCleanup(option, interval: 60);
//            tokenCleanup.Start();*/
//        }
//    }
//}