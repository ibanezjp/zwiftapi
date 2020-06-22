using System;
using System.Collections.Generic;
using System.Text;

namespace Zwift.Functions
{
    class Constants
    {
        public const string DATABASE_NAME = "zwift";
        public const string ROUTES_COLLECTION = "routes";
        public const string USERS_COLLECTION = "users";
        public const string DATABASE_CONNECTION_STRING_SETTING = "COSMOSDB_CONNECTION_STRING";
        public const string QUEUE_NAME = "update-user-routes";
    }
}
