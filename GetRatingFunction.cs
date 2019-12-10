using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace microsoft.gbb
{
    public class GetRatingFunction
	{
        private HttpClient _httpClient;

        public GetRatingFunction(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

		[FunctionName("GetRating")]
		public async Task<IActionResult> GetRating(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Ratings/{ratingId}")] HttpRequest req,
			string ratingId,
			[CosmosDB(databaseName: "%CosmosDbDatabase%", collectionName: "%RatingsContainer%", ConnectionStringSetting = "CosmosDbConnection",
			Id = "{ratingId}", PartitionKey = "{ratingId}")] RatingModel rating,
			ILogger log)
		{
			if (rating != null)
			{
				return new OkObjectResult(rating);
			}

			return new BadRequestObjectResult($"Unable to find rating for {ratingId}");
		}
	}
}
