using Greet;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CancelToken.Client
{
    public class Worker : BackgroundService
    {
        private readonly Random _random;
        private readonly ILogger _logger;
        private readonly Greeter.GreeterClient _client;
        private AsyncClientStreamingCall<HelloRequest, HelloReply> _call;

        public Worker(ILogger<Worker> logger, Greeter.GreeterClient client)
        {
            _random = new Random();
            _logger = logger;
            _client = client;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                source.CancelAfter(5000);

                // 1 cancellationToken
                //await _client.SayHelloAsync(new HelloRequest { Name = _random.Next(1, 10).ToString() }, cancellationToken: source.Token);

                // 2 CallOptions.WithCancellationToken
                //CallOptions callOptions = new CallOptions().WithCancellationToken(source.Token);

                // 3 CallOptions.WithDeadline
                //CallOptions callOptions = new CallOptions().WithDeadline(DateTime.UtcNow.AddSeconds(2));

                // 4 CallOptions.WithHeaders
                CallOptions callOptions = new CallOptions().WithHeaders(new Metadata { new Metadata.Entry("name", "admin") });

                await _client.SayHelloAsync(new HelloRequest { Name = _random.Next(1, 10).ToString() }, callOptions);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Stream deadlineExceeded.");
            }

            //_logger.LogInformation("Starting client streaming call at: {time}", DateTimeOffset.Now);

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    await _call.RequestStream.WriteAsync(new HelloRequest { Name = _random.Next(1, 999).ToString() });

            //    await Task.Delay(1000, stoppingToken);
            //}
        }

        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            _call = _client.SayHellos();

            await base.StartAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            // Tell server that the client stream has finished
            _logger.LogInformation("Finishing call at: {time}", DateTimeOffset.Now);

            await _call.RequestStream.CompleteAsync();

            // Log total
            var response = await _call;

            _logger.LogInformation("Total count: {count}", response.Message);


            await base.StopAsync(stoppingToken);
        }
    }
}
