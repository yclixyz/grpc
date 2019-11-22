using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basic.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Basic.ApiClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Greeter.GreeterClient _client;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILoggerFactory logger, Greeter.GreeterClient client)
        {
            _logger = logger.CreateLogger<WeatherForecastController>();
            _client = client;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var reply = _client.SayHello(new HelloRequest { Name = "Jack" });

            Console.WriteLine(reply.Message);

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
