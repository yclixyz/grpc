using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Progress.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IProgress<int> progress = new Progress<int>(i => Console.WriteLine($"Progress: {i}%"));

            //ReponseReport reponseReport = new ReponseReport(progress);

            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var client = new Progressor.ProgressorClient(channel);

            using var call = client.RunHistory(new Google.Protobuf.WellKnownTypes.Empty());

            await foreach (var item in call.ResponseStream.ReadAllAsync())
            {
                if (item.ResponseTypeCase == HistoryResponse.ResponseTypeOneofCase.Progress)
                {
                    progress.Report(item.Progress);
                }
                else if (item.ResponseTypeCase == HistoryResponse.ResponseTypeOneofCase.Result)
                {
                    Console.WriteLine(item.Result);
                }
            }

            Console.WriteLine("Hello World!");

            Console.ReadLine();
        }
    }
}
