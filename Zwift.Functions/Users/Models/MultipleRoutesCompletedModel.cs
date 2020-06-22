using System.Collections.Generic;

namespace Zwift.Functions.Users.Models
{
    public class MultipleRoutesCompletedModel
    {
        public string UserId { get; set; }
        public List<string> Routes { get; set; }
    }
}
