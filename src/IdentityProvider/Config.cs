// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityProvider
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                 // standard OpenID Connect scopes
                StandardScopes.OpenId,
                StandardScopes.ProfileAlwaysInclude,
                StandardScopes.EmailAlwaysInclude,
           
                // API - access token will 
                // contain roles of user
                new Scope
                {
                    Name = "wanvet",
                    DisplayName = "Scope for the wanvet resource.",
                    Type = ScopeType.Resource,
                    ScopeSecrets = new List<Secret>
                    {
                        new Secret("wanvetSecret".Sha256())
                    },
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role"),
                        new ScopeClaim("wanvet")
                    }
                }           
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientName = "WanVetClientDev",
                    ClientId = "WanVetClientDev",             
                    AccessTokenType = AccessTokenType.Reference,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:44328"

                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:44328/unauthorized"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:44328",
                        "http://localhost:44328"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "wanvet"
                    }
                },
                new Client
                {
                    ClientName = "WanVetClient",
                    ClientId = "WanVetClient",
                    AccessTokenType = AccessTokenType.Reference,
                    //AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "https://localhost/Wanvet"

                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost/Wanvet/unauthorized"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost/Wanvet",
                        "http://localhost/Wanvet"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "wanvet"
                    }
                }
            };
        }
    }
}