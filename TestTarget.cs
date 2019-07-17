using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RESTProxy
{
    public static class TestTarget
    {
        [FunctionName("TestTarget")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route="tester/{date}/getinfo")] HttpRequest req,
            ILogger log, string date)
        {
            try
            {
                log.LogInformation("***Request Recieved***");
                //Write out the HTTP Header values recieved
                log.LogInformation("Headers:");
                foreach(var header in req.Headers)
                {
                    log.LogInformation($"{header.Key} : {header.Value}");
                }
                log.LogInformation("***Sending Response***");
                return (ActionResult)new OkObjectResult($"You sent, {date}");
            }
            catch(Exception ex)
            {
                return (ActionResult)new BadRequestObjectResult($"Failed: {ex.Message}");
            }
        }
    }
}
