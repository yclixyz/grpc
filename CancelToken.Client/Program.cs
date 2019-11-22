using Greet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CancelToken.Client
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddGrpcClient<Greeter.GreeterClient>(options =>
                    {
                        options.Address = new Uri("https://localhost:5001");
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
