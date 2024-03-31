using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

//using Azure;
//using Azure.AI.OpenAI;


namespace DoingAzure.HelloAI
{
    public static class todayinhistory
    {
        [FunctionName("todayinhistory")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Called todayinhistory...");

            string? key = System.Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY", EnvironmentVariableTarget.Process);
            log.LogInformation(key.Length > 0 ? "API Key found" : "API Key not found"); 
            string? endpoint = "https://tdih.openai.azure.com/";
            string? deployment = "tdih-gpt-35-turbo-instruct";
            log.LogInformation($"endpoint = {endpoint}, deployment = {deployment}");
            log.LogInformation($"key = {key}");
            
            // doy is "day of year" - Feb 13, Apr 5, May 1, etc.
            string doy = req.Query["doy"]; 
            log.LogInformation($"doy = {doy}");

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // doy = doy ?? data?.name;

            string year = DateTime.Now.ToString("yyyy");
            
            log.LogInformation($"string.IsNullOrEmpty(doy) = {string.IsNullOrEmpty(doy)}");
            string responseMessage = string.IsNullOrEmpty(doy)
                ? $"This HTTP triggered function executed successfully in year {year}. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {doy} in year {year}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
