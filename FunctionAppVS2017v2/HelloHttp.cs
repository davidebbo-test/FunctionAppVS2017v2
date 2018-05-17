
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System;

namespace FunctionAppVS2017v2
{
    public static class HelloHttp
    {
        static int _counter;

        [FunctionName("HelloHttp")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            TraceWriter log)
        {
            // Static counter to see when new runtime starts
            _counter++;

            log.Info($"C# HTTP trigger function processed a request, Counter={_counter}, Machine={Environment.MachineName}");
            log.Info($"From class library: {MyClassLibrary.Hello.SayHello("David")}");

            string name = req.Query["name"];

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult(new
                {
                    Name = $"Hello, {name}",
                    Time = DateTime.UtcNow,
                    Counter = _counter,
                    Environment.MachineName,
                    req.Host.Host,
                    Foo = Environment.GetEnvironmentVariable("FOO"),
                    Env = Environment.GetEnvironmentVariables()
                })
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
