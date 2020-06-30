using System.Collections.Generic;

namespace gtv.ViewModels
{
    public class Trip
    {
        public string TripTag { get; set; }
        public string Title{ get; set; }
        public string Image { get; set; }
        public string TripHeader { get; set; }
        public List<IContentItem> TripDescription { get; set; }
        public IEnumerable<gtv.Models.TripItem> TripItems { get; set; }
    }

}
