using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Zwift.Functions.Users.Models;

namespace Zwift.Functions.Users
{
    public static class UploadRoutes
    {
        [FunctionName("UploadRoutes")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadRoutes/{UserId}")] HttpRequest httpRequest,
            [CosmosDB(
                Constants.DATABASE_NAME,
                Constants.USERS_COLLECTION,
                ConnectionStringSetting = Constants.DATABASE_CONNECTION_STRING_SETTING,
                CreateIfNotExists = true,
                Id = "{UserId}")] User existingUser,
            //[CosmosDB(
            //    Constants.DATABASE_NAME,
            //    Constants.USERS_COLLECTION,
            //    ConnectionStringSetting = Constants.CONNECTION_STRING_SETTING,
            //    CreateIfNotExists = true,
            //    Id = "{UserId}")] out User user,
            [Blob("pending/{UserId}_{rand-guid}", FileAccess.Write)] Stream image,
            ILogger log)
        {
            byte[] content = new byte[httpRequest.Body.Length];
            await httpRequest.Body.ReadAsync(content, 0, (int)httpRequest.Body.Length);

            await image.WriteAsync(content);

            //if (existingUser == null)
            //{
            //    user = null;
            //    return new BadRequestResult();
            //}

            //var existingRoute = existingUser.PendingRoutes.SingleOrDefault(x => x.Id == model.RouteId);
            //if (existingRoute == null)
            //{
            //    user = null;
            //    return new BadRequestResult();
            //}

            //existingRoute.Completed = DateTime.Now;
            //existingUser.PendingRoutes.Remove(existingRoute);
            //existingUser.CompletedRoutes ??= new List<Route>();
            //existingUser.CompletedRoutes.Add(existingRoute);

            //user = existingUser;
            return new OkResult();
        }
    }
}


//https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string