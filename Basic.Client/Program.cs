using Basic.Server;
using Grpc.Net.Client;
using System;

namespace Basic.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var client = new Greeter.GreeterClient(channel);

            var reply = client.SayHello(new HelloRequest { Name = "Jack" });

            Console.WriteLine(reply.Message);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
