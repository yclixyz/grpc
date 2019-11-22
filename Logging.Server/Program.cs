using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Logging.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
                         Host.CreateDefaultBuilder(args)
                             .ConfigureWebHostDefaults(webBuilder =>
                             {
                                 webBuilder.UseStartup<Startup>();
                             })
                             .UseSerilog((host, logging) =>
                             {
                                 logging.ReadFrom.Configuration(host.Configuration).Enrich.FromLogContext();

                                 logging
                                 .WriteTo.Console()
                                 .WriteTo.File(Path.Combine("logs", "log.txt"), fileSizeLimitBytes: 1_000_000, rollOnFileSizeLimit: true, shared: true, rollingInterval: RollingInterval.Day, flushToDiskInterval: TimeSpan.FromSeconds(1));
                             });
    }
}