using Greet;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace ClientStream.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var client = new Greeter.GreeterClient(channel);

            using (var call = client.SayHellos())
            {
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine($"Client Write {i} ");

                    await call.RequestStream.WriteAsync(new HelloRequest { Name = i.ToString() });

                    await Task.Delay(200);
                }

                await call.RequestStream.CompleteAsync();

                var reply = await call;

                Console.WriteLine(reply.Message);

                Console.WriteLine("Shutting down");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
