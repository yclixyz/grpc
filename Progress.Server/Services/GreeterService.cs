using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Progress.Server
{
    public class GreeterService : Progressor.ProgressorBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task RunHistory(Empty request, IServerStreamWriter<HistoryResponse> responseStream, ServerCallContext context)
        {
            var monarches = await File.ReadAllLinesAsync("Monarchs-of-England.txt");

            var processedMonarches = new List<string>();

            for (int i = 0; i < monarches.Length; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.2));

                processedMonarches.Add(monarches[i]);

                var progress = (i + 1) / (double)monarches.Length;

                await responseStream.WriteAsync(new HistoryResponse { Progress = Convert.ToInt32(progress * 100) });
            }

            var historyResponse = new HistoryResponse();
            historyResponse.Result = new HistoryResult();
            historyResponse.Result.Items.AddRange(processedMonarches);

            await responseStream.WriteAsync(historyResponse);
        }
    }
}
