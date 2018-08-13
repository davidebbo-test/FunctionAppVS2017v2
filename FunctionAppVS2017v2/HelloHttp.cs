
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

            if (name == null)
            {
                return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            }

            // If we're asked to verify that this is a cold start, fail with 500 if it's not
            string verifyColdStart = req.Query["verify_cold_start"];
            if (verifyColdStart != null && _counter > 1)
            {
                return new ObjectResult("Not a cold start request!") { StatusCode = 500 };
            }

            // Verify an env variable if asked to
            string verifyEnvVariable = req.Query["check_env_var"];
            if (verifyEnvVariable != null)
            {
                var keyAndValue = verifyEnvVariable.Split(',');
                if (Environment.GetEnvironmentVariable(keyAndValue[0]) != keyAndValue[1])
                {
                    return new ObjectResult($"Environment variable '{keyAndValue[0]}' is not set to '{keyAndValue[1]}'") { StatusCode = 500 };
                }
            }

            return new OkObjectResult(new
            {
                Name = $"Hello, {name}",
                Time = DateTime.UtcNow,
                Counter = _counter,
                Environment.MachineName,
                req.Host.Host,
                Foo = Environment.GetEnvironmentVariable("FOO"),
                Env = Environment.GetEnvironmentVariables()
            });
        }
    }
}
