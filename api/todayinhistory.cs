using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

#if true
using Azure;
using Azure.AI.OpenAI;
#endif

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

            var key = System.Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY", EnvironmentVariableTarget.Process);
            log.LogInformation(key.Length > 0 ? "API Key found" : "API Key not found");
            var endpoint = "https://tdih.openai.azure.com/";
            var deployment = "tdih-gpt-35-turbo-instruct";
            log.LogInformation($"endpoint = {endpoint}, deployment = {deployment}");

            // doy is "day of year" - Feb 13, Apr 5, May 1, etc.
            string doy = req.Query["doy"];
            log.LogInformation($"doy = {doy}");

#if true
            var client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(key));
            var prompt = "Tell me about an interesting event in world history that took place on this day in some past year. Be sure to include the relevant historical date in the response.";

            if (!string.IsNullOrEmpty(doy))
            {
                var doyServer = DateTime.Now.ToString("MMMM dd");
                if (String.Compare(doy, doyServer, true) != 0)
                {
                    log.LogWarning("Provided doy [{doy}] does not match today's date computed on server [{doyServer}]. Is someone trying to hack us?")
                }
                prompt += $"Today is {doyServer}.\n";
            }
            log.LogInformation($"prompt = {prompt}");

            CompletionsOptions completionsOptions = new()
            {
                DeploymentName = deployment,
                Prompts = { prompt },
                MaxTokens = 250,
            };

            Response<Completions> completionsResponse = client.GetCompletions(completionsOptions);

            #if true
            string completion = completionsResponse.Value.Choices[0].Text;
            #else
            string completion = String.Empty;
            int i = 0;
            foreach (var choice in completionsResponse.Value.Choices)
            {
                if (String.IsNullOrEmpty(completion))
                {
                    completion = choice.Text;
                    log.LogInformation($"capturing completion ({i}): {completion}");
                }
                else
                {
                    log.LogInformation($"ignoring completion ({i}): {completion}");
                }
            }
            #endif
#endif

            log.LogInformation($"\nPROMPT: \n\n{prompt}");
            var today = DateTime.Now.ToString("MMMM dd");
            log.LogInformation($"Today is {today}");
            log.LogInformation($"Tokens used: {completionsResponse.Value.Usage.CompletionTokens}/{completionsOptions.MaxTokens}");


            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // doy = doy ?? data?.name;

            string year = DateTime.Now.ToString("yyyy");

            log.LogInformation($"string.IsNullOrEmpty(doy) = {string.IsNullOrEmpty(doy)}");
            string responseMessage = string.IsNullOrEmpty(doy)
                ? $"This HTTP triggered function executed successfully in year {year}. Pass a name in the query string or in the request body for a personalized response. Otherwise: {completion}"
                : $"Hello, {doy} in year {year}. This HTTP triggered function executed successfully and got this response: {completion}";

            return new OkObjectResult(responseMessage);
        }
    }
}
