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
using Microsoft.Extensions.Configuration;

namespace microsoft.gbb
{
    public class CreateRatingFunction
    {
        private HttpClient _httpClient;

        public CreateRatingFunction(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("CreateRating")]
        public async Task<IActionResult> CreateRating(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Rating")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDbDatabase%", collectionName: "%RatingsContainer%", ConnectionStringSetting = "CosmosDbConnection")]IAsyncCollector<RatingModel> ratingCollector,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            RatingModel ratingModel = JsonConvert.DeserializeObject<RatingModel>(requestBody);

            // verify userId
            string userIdRequest = $"https://serverlessohuser.trafficmanager.net/api/GetUser?userId={ratingModel.userId}";
            var response = await _httpClient.GetAsync(userIdRequest);
            if(!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("UserId is not valid.");
            }

            // verify productId
            string productIdRequest = $"https://serverlessohproduct.trafficmanager.net/api/GetProduct?productId={ratingModel.productId}";
            response = await _httpClient.GetAsync(productIdRequest);            
            if(!response.IsSuccessStatusCode)
            {
                return new BadRequestObjectResult("ProductId is not valid.");
            }

            // verify rating is within 0-5
            if(ratingModel.rating > 5){
                return new BadRequestObjectResult("Rating must be between 0 and 5");
            }


            ratingModel.id = Guid.NewGuid().ToString();
            ratingModel.timestamp = DateTime.UtcNow;

            await ratingCollector.AddAsync(ratingModel);

            return new CreatedResult($"{req.Scheme}://{req.Host}/api/Ratings/{ratingModel.id}", ratingModel);
        }
    }
}

