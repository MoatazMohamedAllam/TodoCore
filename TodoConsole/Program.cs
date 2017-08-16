using IdentityModel.Client;
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

                var client = new DiscoveryClient("http://192.168.43.73:5000");
                client.Policy.RequireHttps = false;              
                
                var disco = await client.GetAsync();
                // request token
                var tokenClient = new TokenClient(disco.TokenEndpoint, "emailpass.client", "secret");
                var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("user", "pass", "api");

                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                    return;
                }

                Console.WriteLine(tokenResponse.Json);


                // call api
                var api = new HttpClient();
                api.SetBearerToken(tokenResponse.AccessToken);

                var response = await api.GetAsync("http://192.168.43.73:5001/api/todo/GetInfo");
                //var response = await client.GetAsync("http://192.168.43.73:5000/connect/userinfo");
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
                Console.WriteLine(ex.Message);
            }

#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif

        }
    }
}
