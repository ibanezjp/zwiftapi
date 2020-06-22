using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Routes
{
    public static class UpdateRoutesOnDemand
    {
        [FunctionName("UpdateRoutesOnDemand")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IAsyncCollector<Route> routes,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IEnumerable<Route> existingRoutes,
            ILogger log)
        {
            try
            {
                var newRoutes = await RoutesHelper.GetRoutesAndUpdateAsync(existingRoutes, routes);

                return new OkObjectResult(newRoutes);
            }
            catch (Exception exception)
            {
                log.LogError(exception, $"Error during {nameof(RoutesScrapper.GetDataAsync)}");
                return new InternalServerErrorResult();
            }
        }
    }
}
