﻿using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TodoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            try
            {
                // discover endpoints from metadata
                var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
                // request token
                var tokenClient = new TokenClient(disco.TokenEndpoint, "emailpass.client", "secret");
                var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("username", "password", "api");

                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return;
                }

                Console.WriteLine(tokenResponse.Json);


                // call api
                var client = new HttpClient();
                client.SetBearerToken(tokenResponse.AccessToken);

                var response = await client.GetAsync("http://localhost:5001/api/todo/GetInfo");
                //var response = await client.GetAsync("http://localhost:5000/connect/userinfo");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(JArray.Parse(content));
                }

            }
            catch (Exception ex)
            {

            }

#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif

        }
    }
}
