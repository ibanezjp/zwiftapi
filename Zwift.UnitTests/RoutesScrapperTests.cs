using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zwift.UnitTests
{
    [TestClass]
    public class RoutesScrapperTests
    {
        [TestMethod]
        public async Task RoutesScrapper_GetDataAsync_OK()
        {
            var routesScrapper = new RoutesScrapper();
            await routesScrapper.GetDataAsync();
            //TODO: Complete
        }
    }
}
