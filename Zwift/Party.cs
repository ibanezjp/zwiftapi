using System;
using System.Collections.Generic;
using System.Text;

namespace Zwift
{
    public class Party
    {
        public List<string> UsersId { get; set; }

        public List<Route> PendingRoutes { get; set; }
    }
}
