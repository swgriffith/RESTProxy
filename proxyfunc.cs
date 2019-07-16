using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RESTProxy
{
    public static class proxyfunc
    {
        public static string target_url;

        //Defines the function. Note that I removed the 'post' options, as we'll only use get
        //I also added 'ExecutionContext context' which will be used to load configuration data
        [FunctionName("proxyfunc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            //Start a new try block. If errors occur in a try block they are 
            //handled by the 'catch' block
            try
            {
            //Write to the log that the function was called
            log.LogInformation("Proxy function call recieved");

            //Call the method to load configuration data
            LoadConfigData(context);

            //Get the current date/time
            var today = DateTime.Today;

            //Remove one month from the current date/time
            var lastMonth = today.AddMonths(-1);

            //Write out the modified date/time to the logs formatted as yyyyMM
            //Note: the $ a the begining of the string lets you plug variables inline into the string
            log.LogInformation($"Last Month: {lastMonth.ToString("yyyyMM")}");

            target_url = target_url.Replace("{date}", lastMonth.ToString("yyyyMM"));

            string resultContent;
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(target_url);
                var result = await client.GetAsync("");
                resultContent = await result.Content.ReadAsStringAsync();
            }

            //Return an OK result (i.e. HTTP 200)
            //also include the response message from the web call
            return (ActionResult)new OkObjectResult($"Call completed: \n {resultContent}");
            }

            //If an error occured in the try block, then catch it and handle it by
            //returning a 'BadRequest' HTTP 400 result with some error detail
            catch(Exception ex)
            {
                return new BadRequestObjectResult(string.Format("Request failed. Error: {0}", ex.Message));
            }
        }

        //Using the execution context, load the data from configuration.
        public static void LoadConfigData(ExecutionContext context)
        {
            //If you're running locally it will use the local.settings.json
            //If you're running in Azure it will use the function app settings
            var config = new ConfigurationBuilder()
            .SetBasePath(context.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
            
            //Pull the settings called TARGET_URL
            target_url = config["TARGET_URL"];
        }
    }
}
