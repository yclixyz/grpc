using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aggregate;
using Count;
using Greet;
using Grpc.Core;

namespace Nested.Server
{
    public class AggregatorService : Aggregator.AggregatorBase
    {
        private readonly Greeter.GreeterClient _greeterClient;
        private readonly Counter.CounterClient _counterClient;

        public AggregatorService(
            Greeter.GreeterClient greeterClient,
            Counter.CounterClient counterClient)
        {
            _greeterClient = greeterClient;
            _counterClient = counterClient;
        }

        public override async Task<CounterReply> AccumulateCount(IAsyncStreamReader<CounterRequest> requestStream, ServerCallContext context)
        {
            // Forward the call on to the counter service
            using (var call = _counterClient.AccumulateCount())
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    await call.RequestStream.WriteAsync(message);
                }

                await call.RequestStream.CompleteAsync();
                return await call;
            }
        }

        public override async Task SayHellos(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            // Forward the call on to the greeter service
            using (var call = _greeterClient.SayHellos(request))
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    await responseStream.WriteAsync(message);
                }
            }
        }
    }
}
