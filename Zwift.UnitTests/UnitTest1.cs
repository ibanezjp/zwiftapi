using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace Zwift.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var routesScrapper = new RoutesScrapper();
            await routesScrapper.GetDataAsync();
        }

        [TestMethod]
        public async Task TestMethod2()
        {
            var routes = new RoutesList();
            routes.AddRange(new [] {
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
