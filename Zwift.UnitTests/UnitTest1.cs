using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
