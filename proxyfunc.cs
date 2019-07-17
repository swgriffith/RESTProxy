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


            //Replace the {date} parameter in the URL string with the actual date to be passed
            target_url = target_url.Replace("{date}", lastMonth.ToString("yyyyMM"));

            string resultContent;

            //This 'using' block basically makes sure that the client object is cleaned up
            //after the code completes, so you dont have connections held open
            using(var client = new HttpClient())
            {
                //Build the request, adding a few sample header values and the content type, which is usually application/json for a REST call
                //Note that if you're calling something that expects a Bearer token that is handled specially as shown below.
                client.BaseAddress = new Uri(target_url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "insert_your_token_here");
                client.DefaultRequestHeaders.Add("SOMEHEADER1","SomeData1");
                client.DefaultRequestHeaders.Add("SOMEHEADER2","SomeData2");
                
                //Make the web request
                var result = await client.GetAsync("");

                //Read the response data into a string to be used later
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
