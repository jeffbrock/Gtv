using System;
using System.ComponentModel.DataAnnotations;

namespace gtv.Models
{
    public class TripItem
    {
        [Required]
        public string Title { get; set; }
        public string TripTag { get; set; }
        public string Tag { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public bool Published { get; set; }
        public string SubTitle { get; set; }
        public string Author { get; set; }
        public string HeaderImage { get; set; }
        public string TripContent { get; set; }
        public string LessonTag { get; set; }
    }
}
