using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace Zwift.Functions.Routes
{
    public class RoutesHelper
    {
        public static async Task<List<Route>> GetRoutesAndUpdateAsync(IEnumerable<Route> existingRoutes, IAsyncCollector<Route> routes)
        {
            var sourceRoutes = await new RoutesScrapper().GetDataAsync();

            var newRoutes = sourceRoutes
                .Where(x => !existingRoutes.Select(route => route.Id).Contains(x.Id))
                .ToList();

            foreach (var newRoute in newRoutes) 
                await routes.AddAsync(newRoute);

            await routes.FlushAsync();

            return newRoutes;
        }

        public static async Task<List<Route>> GetRoutesAndCreateAsync(IAsyncCollector<Route> routes)
        {
            var sourceRoutes = await new RoutesScrapper().GetDataAsync();

            foreach (var newRoute in sourceRoutes)
                await routes.AddAsync(newRoute);

            await routes.FlushAsync();

            return sourceRoutes;
        }
    }
}
