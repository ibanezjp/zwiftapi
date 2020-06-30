using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace Zwift.UnitTests
{
    [TestClass]
    public class RouteTests
    {
        [TestMethod]
        public async Task Route_FindRoute_OK()
        {
            var routes = new RoutesList();
            routes.AddRange(new[] {
                new Route
                {
                    Name = "Classique"
                },
                new Route()
                {
                    Name = "London"
                },
                new Route()
                {
                    Name = "Mountain"
                }
            });

            var route = routes.FindRoute("London Classique");
            route.Name.Should().Be.EqualTo("Classique");
        }
    }
}
