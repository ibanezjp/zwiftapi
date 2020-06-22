using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace Zwift.UnitTests
{
    [TestClass]
    public class RoutesManagerTests
    {
        [TestMethod]
        public void GetPendingRoutes_OK()
        {
            #region Routes 

            var route1 = new Route
            {
                Id = "1"
            };

            var route2 = new Route
            {
                Id = "2"
            };

            var route3 = new Route
            {
                Id = "3"
            };

            var route4 = new Route
            {
                Id = "4"
            };

            #endregion

            var user1 = new User
            {
                PendingRoutes = new List<Route>
                {
                    route1,
                    route2
                }
            };

            var user2 = new User
            {
                PendingRoutes = new List<Route>
                {
                    route1,
                    route2,
                    route3
                }
            };

            var user3 = new User
            {
                PendingRoutes = new List<Route>
                {
                    route2,
                    route3,
                    route4
                }
            };

            var pendingRoutes = RoutesManager.GetPendingRoutes(new List<User>
            {
                user1, user2, user3
            });

            pendingRoutes.Count.Should().Be.EqualTo(1);
            pendingRoutes.First().Id.Should().Be.EqualTo("2");
        }
    }
}
