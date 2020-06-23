using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Zwift.Functions.Users.Models;

namespace Zwift.Functions.Users
{
    public static class MarkRoutesAsCompleted
    {
        [FunctionName("MarkRoutesAsCompleted")]
        public static void Run([QueueTrigger(Constants.QUEUE_NAME)]MultipleRoutesCompletedModel multipleRoutesCompletedModel,
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
            foreach (var route in multipleRoutesCompletedModel.Routes)
            {
                var existingRoute = existingUser.PendingRoutes.FindRoute(route);

                if (existingRoute != null)
                {
                    existingRoute.Completed = DateTime.Now;
                    existingUser.PendingRoutes.Remove(existingRoute);
                    existingUser.CompletedRoutes ??= new RoutesList();
                    existingUser.CompletedRoutes.Add(existingRoute);
                }
                else
                {
                    var tmp = existingUser.CompletedRoutes.SingleOrDefault(x => x.Name.Equals(route, StringComparison.InvariantCultureIgnoreCase));
                    if(tmp == null)
                        log.LogInformation($"Route: {route} not found!");
                }
            }
            user = existingUser;
        }
    }
}
