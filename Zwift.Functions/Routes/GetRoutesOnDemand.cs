using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Zwift.Functions.Routes
{
    public static class GetRoutesOnDemand
    {
        [FunctionName("GetRoutesOnDemand")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var routes = await new RoutesScrapper().GetDataAsync();
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
