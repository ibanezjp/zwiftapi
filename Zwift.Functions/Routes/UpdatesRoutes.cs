using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Routes
{
    public static class UpdatesRoutes
    {
        //[FunctionName("UpdateRoutes")]
        public static async void Run(
            [TimerTrigger("0 0 0 1 * *", RunOnStartup = false)] TimerInfo myTimer,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IAsyncCollector<Route> routes,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IEnumerable<Route> existingRoutes,
            ILogger log)
        {
            log.LogInformation($"{nameof(UpdatesRoutes)} Timer trigger function started at: {DateTime.Now}");

            await RoutesHelper.GetRoutesAndUpdateAsync(existingRoutes, routes);
        }
    }
}
