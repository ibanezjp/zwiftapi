using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Users
{
    public static class AddUser
    {
        [FunctionName("AddUser")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] User newUser,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IAsyncCollector<User> usersCollection,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{Id}")] User existingUser,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.ROUTES_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true)] IEnumerable<Route> existingRoutes,
            ILogger log)
        {
            if (existingUser != null) 
                return new BadRequestErrorMessageResult("Id already exists!");

            newUser.PendingRoutes = existingRoutes.ToList();

            await usersCollection.AddAsync(newUser);
            await usersCollection.FlushAsync(); 
            return new OkObjectResult(newUser);
        }
    }
}
