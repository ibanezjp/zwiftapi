using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Users
{
    public static class UploadRoutes
    {
        [FunctionName("UploadRoutes")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UploadRoutes/{UserId}")] HttpRequest httpRequest,
            [Blob("pending/{UserId}_{rand-guid}", FileAccess.Write)] Stream image,
            ILogger log)
        {
            byte[] content = new byte[httpRequest.Body.Length];
            await httpRequest.Body.ReadAsync(content, 0, (int)httpRequest.Body.Length);
            return new OkResult();
        }
    }
}

//https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string