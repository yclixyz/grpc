﻿using Grpc.Core;
using Grpc.Net.Client;
using Mail;
using System;
using System.Threading.Tasks;

namespace Bidirectional.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var mailboxName = GetMailboxName(args);

            Console.WriteLine($"Creating client to mailbox '{mailboxName}'");
            Console.WriteLine();

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Mailer.MailerClient(channel);

            Console.WriteLine("Client created");
            Console.WriteLine("Press escape to disconnect. Press any other key to forward mail.");

            using (var call = client.MailBox(headers: new Metadata { new Metadata.Entry("mailbox-name", mailboxName) }))
            {
                var responseTask = Task.Run(async () =>
                {
                    await foreach (var message in call.ResponseStream.ReadAllAsync())
                    {
                        Console.ForegroundColor = message.Reason == Reason.Received ? ConsoleColor.White : ConsoleColor.Green;
                        Console.WriteLine();
                        Console.WriteLine(message.Reason == Reason.Received ? "Mail received" : "Mail forwarded");
                        Console.WriteLine($"New mail: {message.New}, Forwarded mail: {message.Forwarded}");
                        Console.ResetColor();
                    }
                });

                while (true)
                {
                    var result = Console.ReadKey(intercept: true);
                    if (result.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                    await call.RequestStream.WriteAsync(new ForwardMailMessage());
                }

                Console.WriteLine("Disconnecting");
                await call.RequestStream.CompleteAsync();
                await responseTask;
            }

            Console.WriteLine("Disconnected. Press any key to exit.");
            Console.ReadKey();
        }

        private static string GetMailboxName(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("No mailbox name provided. Using default name. Usage: dotnet run <name>.");
                return "DefaultMailbox";
            }

            return args[0];
        }
    }
}
