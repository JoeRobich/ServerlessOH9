using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;

namespace microsoft.gbb
{
    public class GetRatingsFunction
    {
                private HttpClient _httpClient;

        public GetRatingsFunction(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("GetRatings")]
        public async Task<IActionResult> GetRatings(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AllRatings/{userId}")] HttpRequest req,
			[CosmosDB(databaseName: "%CosmosDbDatabase%", collectionName: "%RatingsContainer%", ConnectionStringSetting = "CosmosDbConnection")] IEnumerable<RatingModel> ratings,
            ILogger log)
        {
             if (ratings != null && ratings.Any())
			{
				return new OkObjectResult(ratings);
			}

			return new BadRequestObjectResult($"No ratings found in database");
        }
    }
}
