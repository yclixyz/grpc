using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Greet;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace CancelToken.Server
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var header = context.RequestHeaders.FirstOrDefault(c => c.Key == "name");

            Console.WriteLine($"header:{header?.Value}");

            var count = 0;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                Interlocked.Increment(ref count);

                Console.WriteLine(count);
            }

            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {count}" + request.Name
            });
        }

        public override async Task<HelloReply> SayHellos(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            List<string> names = new List<string>();

            await foreach (var item in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"Server Read {item} ");
                names.Add(item.Name);
            }

            return new HelloReply
            {
                Message = string.Join(",", names)
            };
        }
    }
}
