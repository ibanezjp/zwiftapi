using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Routes
{
    public static class UpsertRoutesOnDemand
    {
        [FunctionName("UpsertRoutesOnDemand")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] DocumentClient documentClient,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IEnumerable<Route> existingRoutes,
            ILogger log)
        {
            try
            {
                var routes = await RoutesHelper.GetRoutesToUpsertAsync(existingRoutes);

                var collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DATABASE_NAME, Constants.ROUTES_COLLECTION);

                foreach (var route in routes)
                {
                    await documentClient.UpsertDocumentAsync(collectionUri, route);
                }

                return new OkObjectResult(routes);
            }
            catch (Exception exception)
            {
                log.LogError(exception, $"Error during {nameof(RoutesScrapper.GetDataAsync)}");
                return new InternalServerErrorResult();
            }
        }
    }
}
