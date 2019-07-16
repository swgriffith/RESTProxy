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
                return (ActionResult)new OkObjectResult($"You sent, {date}");
            }
            catch(Exception ex)
            {
                return (ActionResult)new BadRequestObjectResult($"Failed: {ex.Message}");
            }
        }
    }
}
