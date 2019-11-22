using Aggregate;
using Count;
using Greet;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nested.Client
{
    public class Program
    {
        static Random RNG = new Random();

        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Aggregator.AggregatorClient(channel);

            await ServerStreamingCallExample(client);

            await ClientStreamingCallExample(client);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task ServerStreamingCallExample(Aggregator.AggregatorClient client)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3.5));

            using (var replies = client.SayHellos(new HelloRequest { Name = "AggregatorClient" }, cancellationToken: cts.Token))
            {
                try
                {
                    await foreach (var message in replies.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine("Greeting: " + message.Message);
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Stream cancelled.");
                }
            }
        }

        private static async Task ClientStreamingCallExample(Aggregator.AggregatorClient client)
        {
            using (var call = client.AccumulateCount())
            {
                for (var i = 0; i < 3; i++)
                {
                    var count = RNG.Next(5);
                    Console.WriteLine($"Accumulating with {count}");
                    await call.RequestStream.WriteAsync(new CounterRequest { Count = count });
                    await Task.Delay(2000);
                }

                await call.RequestStream.CompleteAsync();

                var response = await call;
                Console.WriteLine($"Count: {response.Count}");
            }
        }
    }
}
