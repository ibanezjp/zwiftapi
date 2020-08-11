using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Users
{
    public static class UpdateUsersRouteData
    {
        [FunctionName("UpdateUsersRouteData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest httpRequest,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IEnumerable<Route> existingRoutes,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] DocumentClient client,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IEnumerable<User> users,
            ILogger log)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DATABASE_NAME, Constants.USERS_COLLECTION);

            foreach (var user in users)
            {
                foreach (var route in existingRoutes)
                {
                    user.PendingRoutes ??= new RoutesList();
                    user.CompletedRoutes ??= new RoutesList();

                    var routeInPendingRoutes = user.PendingRoutes.SingleOrDefault(x => x.Id == route.Id);
                    if (routeInPendingRoutes == null)
                    {
                        var routeInCompletedRoutes = user.CompletedRoutes.SingleOrDefault(x => x.Id == route.Id);

                        if (routeInCompletedRoutes == null)
                        {
                            user.PendingRoutes.Add(route);
                        }
                        else
                        {
                            UpdateRouteData(route, routeInCompletedRoutes);
                        }
                    }
                    else
                    {
                        UpdateRouteData(route, routeInPendingRoutes);
                    }
                }
                await client.UpsertDocumentAsync(collectionUri, user);
            }
            return new OkResult();
        }

        private static void UpdateRouteData(Route route, Route userRouteData)
        {
            userRouteData.AllowedSports = route.AllowedSports;
            userRouteData.Distance = route.Distance;
            userRouteData.DistanceFromStart = route.DistanceFromStart;
            userRouteData.EventOnly = route.EventOnly;
            userRouteData.Gain = route.Gain;
            userRouteData.HasAward = route.HasAward;
            userRouteData.MinLevel = route.MinLevel;
            userRouteData.Name = route.Name;
            userRouteData.World = route.World;
        }
    }
}
