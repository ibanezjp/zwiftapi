using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Zwift.Functions.Users.Models;

namespace Zwift.Functions.Users
{
    public static class RemoveCompletedRoute
    {
        [FunctionName("RemoveCompletedRoute")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] RouteCompletedModel model,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{UserId}",
                PartitionKey = "{UserId}")] User existingUser,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{UserId}",
                PartitionKey = "{UserId}")] out User user,
            ILogger log)
        {
            if (existingUser == null)
            {
                user = null;
                return new BadRequestResult();
            }

            var existingRoute = existingUser.CompletedRoutes.SingleOrDefault(x => x.Id == model.RouteId);
            if (existingRoute == null)
            {
                user = null;
                return new BadRequestResult();
            }

            existingRoute.Completed = null;
            existingUser.CompletedRoutes.Remove(existingRoute);
            existingUser.PendingRoutes.Add(existingRoute);

            user = existingUser;
            return new OkResult();
        }
    }
}
