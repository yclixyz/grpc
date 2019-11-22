using Greet;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Authentication.Client
{
    class Program
    {
        private const string Address = "https://localhost:5001";

        static async Task Main(string[] args)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{Address}/token?name={HttpUtility.UrlEncode(Environment.UserName)}"),
                Method = HttpMethod.Get,
                Version = new Version(2, 0)
            };
            var tokenResponse = await httpClient.SendAsync(request);

            if (tokenResponse.IsSuccessStatusCode)
            {
                var token = await tokenResponse.Content.ReadAsStringAsync();

                Metadata headers = null;

                if (token != null)
                {
                    headers = new Metadata
                    {
                        { "Authorization", $"Bearer {token}" }
                    };
                }

                var channel = GrpcChannel.ForAddress(Address);

                var client = new Greeter.GreeterClient(channel);

                var reply = client.SayHello(new HelloRequest { Name = "Jack" }, headers);

                Console.WriteLine(reply.Message);

                Console.WriteLine("Hello World!");
            }
        }
    }
}
