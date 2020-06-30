using System.Collections.Generic;

namespace gtv.ViewModels
{
    public class AdminTripItem
    {
        public Models.TripItem TripItem { get; set;}
        public ICollection<string> Images { get; set; }
        public List<Authors> Authors { get; set; }
    }

    public class Authors
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
    }
}
