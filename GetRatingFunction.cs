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
	public static class GetRatingFunction
	{
		private static HttpClient _httpClient = new HttpClient();

		[FunctionName("GetRating")]
		public static async Task<IActionResult> GetRating(
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
