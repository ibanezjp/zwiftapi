using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Zwift.Functions.Users.Models;

namespace Zwift.Functions.Users
{
    public static class AddCompletedRoute
    {
        [FunctionName("AddCompletedRoute")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] RouteCompletedModel model,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{UserId}")] User existingUser,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{UserId}")] out User user,
            ILogger log)
        {
            if (existingUser == null)
            {
                user = null;
                return new BadRequestResult();
            }

            var existingRoute = existingUser.PendingRoutes.SingleOrDefault(x => x.Id == model.RouteId);
            if (existingRoute == null)
            {
                user = null;
                return new BadRequestResult();
            }

            existingRoute.Completed = DateTime.Now;
            existingUser.PendingRoutes.Remove(existingRoute);
            existingUser.CompletedRoutes ??= new List<Route>();
            existingUser.CompletedRoutes.Add(existingRoute);

            user = existingUser;
            return new OkResult();
        }
    }
}
