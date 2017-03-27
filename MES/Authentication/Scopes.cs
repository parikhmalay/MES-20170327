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
using System.Collections.Generic;
using System.Linq;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace MES.Authentication
{
    public class Scopes
    {
        public static IEnumerable<Scope> Get()
        {

            return new Scope[]
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                StandardScopes.Phone,
                StandardScopes.Address,
                StandardScopes.Roles,
                StandardScopes.OfflineAccess,
                new Scope
                {
                    Enabled = true,
                    Name = "roles",
                    Type = ScopeType.Identity,
                    IncludeAllClaimsForUser = true,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim(Constants.ClaimTypes.Role)
                        
                    }
                },
                new Scope
                {
                    Enabled = true,
                    Name = "Api",
                    Description = "Access to an API",
                    Type = ScopeType.Resource,
                    IncludeAllClaimsForUser = true,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role"),
                        new ScopeClaim(Constants.ClaimTypes.FamilyName),
                        new ScopeClaim(Constants.ClaimTypes.GivenName),
                        new ScopeClaim(Constants.ClaimTypes.NickName),
                    }
                }
                
             };
        }
    }
}




//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Thinktecture.IdentityServer.Core.Models;

//namespace IdentityServer.IdentityServer
//{
//    public static class Scopes
//    {
//        public static IEnumerable<Scope> Get()
//        {
//            var scopes = new List<Scope>
//        {
//            new Scope
//            {
//                Enabled = true,
//                Name = "roles",
//                Type = ScopeType.Identity,
//                Claims = new List<ScopeClaim>
//                {
//                    new ScopeClaim("role")
//                }
//            },
//            new Scope
//            {
//                Enabled = true,
//                Name = "sampleApi",
//                Description = "Access to a sample API",
//                Type = ScopeType.Resource,
//                Claims = new List<ScopeClaim>
//                {
//                    new ScopeClaim("role") 
//                }
//            }
//        };

//            scopes.AddRange(StandardScopes.All);

//            return scopes;
//        }
//    }
//}