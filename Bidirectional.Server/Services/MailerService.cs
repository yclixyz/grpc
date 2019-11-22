using Grpc.Core;
using Mail;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidirectional.Server
{
    public class MailerService : Mailer.MailerBase
    {
        private readonly MailQueue _mailQueue;
        private readonly ILogger _logger;

        public MailerService(MailQueue mailQueue, ILoggerFactory loggerFactory)
        {
            _mailQueue = mailQueue;
            _logger = loggerFactory.CreateLogger<MailerService>();
        }

        public override async Task MailBox(IAsyncStreamReader<ForwardMailMessage> requestStream, IServerStreamWriter<MailBoxMessage> responseStream, ServerCallContext context)
        {
            var mailboxName = context.RequestHeaders.Single(c => c.Key == "mailbox-name").Value;

            _logger.LogInformation($"Connected to {mailboxName}");

            _mailQueue.Changed += _mailQueue_Changed;

            _mailQueue.Received();

            try
            {
                await foreach (var item in requestStream.ReadAllAsync())
                {
                    if (_mailQueue.TryForwardMail(out var message))
                    {
                        _logger.LogInformation($"Forwarded mail: {message.Content}");
                    }
                    else
                    {
                        _logger.LogWarning("No mail to forward.");
                    }
                }
            }
            finally
            {
                _mailQueue.Changed -= _mailQueue_Changed;
            }

            _logger.LogInformation($"{mailboxName } disconnected");



            await Task.CompletedTask;

            async Task _mailQueue_Changed((int totalCount, int fowardCount, Reason reason) state)
            {
                await responseStream.WriteAsync(new MailBoxMessage
                {
                    Forwarded = state.totalCount,
                    New = state.totalCount - state.fowardCount,
                    Reason = state.reason
                });
            }
        }
    }
}
