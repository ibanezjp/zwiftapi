using System;
using System.Collections.Generic;
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
                ConnectionStringSetting = Constants.CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{UserId}")] User existingUser,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{UserId}")] out User user,
            ILogger log)
        {
            foreach (var route in multipleRoutesCompletedModel.Routes)
            {
                var existingRoute = existingUser.PendingRoutes.SingleOrDefault(x => x.Name.Equals(route, StringComparison.InvariantCultureIgnoreCase));

                if (existingRoute != null)
                {
                    existingRoute.Completed = DateTime.Now;
                    existingUser.PendingRoutes.Remove(existingRoute);
                    existingUser.CompletedRoutes ??= new List<Route>();
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