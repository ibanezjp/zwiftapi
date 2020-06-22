using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Users
{
    public static class GetPendingRoutes
    {
        [FunctionName("GetPendingRoutes")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.CONNECTION_STRING_SETTING)] DocumentClient client,
            ILogger log)
        {
            var ids = req.Query["ids"];
            if (string.IsNullOrWhiteSpace(ids))
            {
                return new NotFoundResult();
            }

            var min = req.Query["min"];
            var max = req.Query["max"];
            var world = req.Query["world"];

            var userIds = ids.First().Split(',');

            var collectionUri = UriFactory.CreateDocumentCollectionUri(Constants.DATABASE_NAME, Constants.USERS_COLLECTION);

            var query = client.CreateDocumentQuery<User>(collectionUri)
                .Where(x => userIds.Contains(x.Id))
                .AsDocumentQuery();

            var users = new List<User>();

            while (query.HasMoreResults)
            {
                foreach (User user in await query.ExecuteNextAsync<User>())
                {
                    users.Add(user);
                }
            }

            var pendingRoutes = RoutesManager.GetPendingRoutes(users);

            if (!string.IsNullOrWhiteSpace(min))
            {
                pendingRoutes = pendingRoutes.Where(x => x.Distance > int.Parse(min.First())*1000).ToList();
            }

            if (!string.IsNullOrWhiteSpace(max))
            {
                pendingRoutes = pendingRoutes.Where(x => x.Distance < int.Parse(max.First())*1000).ToList();
            }

            if (!string.IsNullOrWhiteSpace(world))
            {
                pendingRoutes = pendingRoutes.Where(x => x.World.Equals(world.First(),StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            return new OkObjectResult(pendingRoutes);
        }
    }
}
