using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionAppVS2017v2
{
    public static class MyTimer
    {
        static int _counter;

        [FunctionName("MyTimer")]
        public static void Run(
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Queue("myqueue-items-destination")] out string myQueueItemCopy,
            TraceWriter log)
        {
            ++_counter;
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}, Counter={_counter}, Machine={Environment.MachineName}");
            myQueueItemCopy = $"{DateTime.Now}, Counter={_counter}, Site={Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")} Machine={Environment.MachineName}";
        }
    }
}
