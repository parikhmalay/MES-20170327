/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using System.Configuration;



namespace MES.Authentication
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
               new Client
                {
                    ClientName = "MES",
                    Enabled = true,
                    ClientId = "MES",
                    ClientSecrets = new List<Secret>{
                        new Secret("secret".Sha256())
                    },
                    Flow = Flows.Implicit,
                    
                    RequireConsent = false,
                    AllowRememberConsent = false,
                    //IdentityProviderRestrictions = new List<string>{"idsrv"},
                    RedirectUris = new List<string>
                    {
                        "http://localhost:65389",
                        "http://localhost:81",
                        "http://192.168.10.3:81",
                        ConfigurationManager.AppSettings["RedirectUri"],
                        ConfigurationManager.AppSettings["RedirectUri1"]


                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:65389",
                        "http://localhost:81",
                        "http://192.168.10.3:81",
                        ConfigurationManager.AppSettings["RedirectUri"],
                        ConfigurationManager.AppSettings["RedirectUri1"]
                    },
                    AllowedScopes = new List<string>
                    { 
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Phone,
                        "names",
                        "Api",
                        "roles"
                    },

                    AccessTokenType = AccessTokenType.Jwt,
                    IdentityTokenLifetime = 36000,
                    AccessTokenLifetime = 36000,
                },
               
            };
        }
    }
}

