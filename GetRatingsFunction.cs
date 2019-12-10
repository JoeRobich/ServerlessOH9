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

namespace microsoft.gbb
{
    public static class GetRatingsFunction
    {
        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("GetRatings")]
        public static async Task<IActionResult> GetRatings(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AllRatings")] HttpRequest req,
			string ratingId,
			[CosmosDB(databaseName: "%CosmosDbDatabase%", collectionName: "%RatingsContainer%", ConnectionStringSetting = "CosmosDbConnection", PartitionKey = "{ratingId}")] List<RatingModel> ratings,
            ILogger log)
        {
             if (ratings != null && ratings.Count > 0)
			{
				return new OkObjectResult(ratings);
			}

			return new BadRequestObjectResult($"No ratings found in database");
        }
    }
}
