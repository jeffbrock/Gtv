using System;

namespace gtv.ViewModels
{
    public class HomeTripItem
    {
        public string Title { get; set; }
        public string TripTitle { get; set; }
        public string TripTag { get; set; }
        public string Tag { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string SubTitle { get; set; }
        public string HeaderImage { get; set; }
    }

}
