using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Channels;
using System.Threading;
using Grpc.Core;
using Mail;
using System.Collections.Concurrent;

namespace Bidirectional.Server
{
    public class MailQueue
    {
        private Channel<Mail> _channel;
        private int _totalCount;
        private int _forwardCount;
        public event Func<(int, int, Reason), Task> Changed;

        public MailQueue()
        {
            _channel = Channel.CreateUnbounded<Mail>();
        }

        public void Received()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    Interlocked.Increment(ref _totalCount);

                    var content = $"#{_totalCount}Message";

                    await _channel.Writer.WriteAsync(new Mail { Id = _totalCount, Content = content });

                    OnChange(Reason.Received);

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }

        public bool TryForwardMail(out Mail message)
        {
            if (_channel.Reader.TryRead(out message))
            {
                Interlocked.Increment(ref _forwardCount);
                OnChange(Reason.Forwarded);

                return true;
            }

            return false;
        }

        public void OnChange(Reason reason)
        {
            Changed.Invoke((_totalCount, _forwardCount, reason));
        }
    }
}
