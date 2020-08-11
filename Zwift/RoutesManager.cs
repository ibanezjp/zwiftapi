using System.Collections.Generic;
using System.Linq;
using deepequalitycomparer;

namespace Zwift
{
    public class RoutesManager
    {
        public static List<Route> GetPendingRoutes(List<User> users)
        {
            if(users == null || users.Count == 0)
                return new List<Route>();

            var pendingRoutesByUsers = users.Select(x => x.PendingRoutes).ToList();

            var intersection = pendingRoutesByUsers
                .Skip(1)
                .Aggregate(
                    new HashSet<string>(pendingRoutesByUsers.First().Select(x => x.Id)),
                    (h, e) => { h.IntersectWith(e.Select(x => x.Id)); return h; }
                );

            return users.First().PendingRoutes.Where(x => intersection.Contains(x.Id)).ToList();
        }

        public static List<Route> GetRoutesToUpdate(List<Route> existingRoutes, List<Route> routes)
        {
            var comparer = DeepEqualityComparer.CraeteDefaultWithLogOnlyNotEqualToConsole<Route>(); 

            return existingRoutes.Except(routes, comparer)
                .Union(routes.Except(existingRoutes, comparer))
                .ToList();
        }
    }
}
