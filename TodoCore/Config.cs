using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoCore
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api") {
                     UserClaims = { "role" }
                },
                new ApiResource("data")
                {
                    ApiSecrets =
                    {
                        new Secret("dataSecret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "datascope",
                            DisplayName = "Scope for the data ApiResource"
                        }


                    },
                     UserClaims = { "role" }
                    //UserClaims = { "role", "admin", "user", "dataEventRecords", "dataEventRecords.admin", "dataEventRecords.user" }
                }
            };
        }

        //defining identity resources
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("datascope",new []{ "role"} ),
            };
        }

        //defining list of clients and which resource they can access
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "emailpass.client",
                    ClientName = "API Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        "api",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                },
                 new Client
                {
                    ClientId = "angular.client",
                    ClientName = "Angular Client",
                    AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = 864000,
                    AlwaysIncludeUserClaimsInIdToken = true,

                    RedirectUris = {"http://localhost:3000"},
                    PostLogoutRedirectUris = { "http://localhost:3000"},
                    AllowedCorsOrigins = {  "http://localhost:3000" },


                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "data",
                        "datascope",
                        "offline_access"
                    },

                    RequireConsent=false,
                    AllowOfflineAccess = true
                }
            };
        }


    }
}
