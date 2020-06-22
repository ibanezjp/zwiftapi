using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Zwift
{
    public class RoutesScrapper
    {
        public async Task<List<Route>> GetDataAsync()
        {
            var url = "https://zwifthacks.com/app/routes/";
            var doc = await new HtmlWeb().LoadFromWebAsync(url);
            var routes = new List<Route>();

            //https://github.com/atifaziz/Fizzler
            var routeNodes = doc.DocumentNode.QuerySelectorAll("div.item.columns.py-2");

            foreach (var routeNode in routeNodes)
            {
                var routeId = routeNode.QuerySelector("div.secret.ze-col-id").InnerText;
                var routeName = routeNode.QuerySelector("div.secret.ze-col-route").InnerText;
                var routeWorld = routeNode.QuerySelector("div.secret.ze-col-world").InnerText;
                var routeDistance = routeNode.QuerySelector("div.secret.ze-col-distance").InnerText;
                var routeGain = routeNode.QuerySelector("div.secret.ze-col-gain").InnerText;
                var routeDistanceFromStart = routeNode.QuerySelector("div.secret.ze-col-distancefromstart").InnerText;
                var routeAward = routeNode.QuerySelector("i.fad.fa-award");
                var routeAllowedSports = routeNode.QuerySelector("div.ze-col-sports").InnerText;
                var routeMinLevel = routeNode.QuerySelector("div.ze-col-levellock").InnerText;
                var routeEventOnly = routeNode.QuerySelector("div.ze-col-eventonly").InnerText;

                routes.Add(new Route
                {
                    Id = routeId,
                    Name = routeName,
                    World = routeWorld,
                    Distance = int.Parse(routeDistance),
                    DistanceFromStart = int.Parse(routeDistanceFromStart),
                    Gain = int.Parse(routeGain),
                    EventOnly = routeEventOnly.Equals("Event only", StringComparison.InvariantCultureIgnoreCase),
                    HasAward = routeAward != null,
                    MinLevel = routeMinLevel.Equals("-", StringComparison.InvariantCultureIgnoreCase) ? 0 : int.Parse(routeMinLevel),
                    AllowedSports = routeAllowedSports.Equals("All", StringComparison.InvariantCultureIgnoreCase) ?
                        Sports.Cycling | Sports.Running :
                        routeAllowedSports.Equals("Running", StringComparison.InvariantCultureIgnoreCase) ?
                            Sports.Running :
                            Sports.Cycling
                });
            }

            return routes;
        }
    }
}
