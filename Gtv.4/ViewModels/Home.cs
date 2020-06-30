using System.Collections.Generic;

namespace gtv.ViewModels
{
    public class Home
    {
        public IEnumerable<Models.Trip> Carousels { get; set; }
        public IEnumerable<HomeTripItem> TripItems { get; set; }
    }

}
