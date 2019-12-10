using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace microsoft.gbb
{
    public static class CreateRatingFunction
    {
        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("CreateRating")]
        public static async Task<IActionResult> CreateRating(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Rating")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RatingModel model = JsonConvert.DeserializeObject<RatingModel>(requestBody);

            string userIdRequest = $"https://serverlessohuser.trafficmanager.net/api/GetUser?userId={model.userId}";
            var response = await _httpClient.GetAsync(userIdRequest);
            if(!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("UserId is not valid.");
            }

            string productIdRequest = $"https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={model.productId}";
            response = await _httpClient.GetAsync(productIdRequest);
            if(!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("ProductId is not valid.");
            }

            model.id = Guid.NewGuid().ToString();
            model.timestamp = DateTime.UtcNow;

            return new CreatedResult($"http://localhost:7071/api/Ratings/{model.id}", model);
        }
    }
}

