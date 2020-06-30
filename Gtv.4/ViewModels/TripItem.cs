using System;
using System.Collections.Generic;

namespace gtv.ViewModels
{
    public class TripItem
    {
        public int Id { get; set; }
        public string TripTag { get; set; }
        public string TripItemTag { get; set; }
        public DateTimeOffset PublishDate { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Author { get; set; }
        public string HeaderImage { get; set; }
        public List<IContentItem> TripContent { get; set; }
        public string LessonTag { get; set; }
    }

    public enum CurrentItem
    {
        text, image, list, notset
    }


    public enum TextType
    {
        bold, italic, text
    }

    public interface IContentItem { }

    public class ListItem : IContentItem
    {
        public List<string> Items { get; set; }
    }

    public class LargeImageItem : IContentItem
    {
        public string Image { get; set; }
        public string Caption { get; set; }
    }

    public class MediumImageItem : IContentItem
    {
        public string Image { get; set; }
        public string Caption { get; set; }
    }

    public class TextItem
    {
        public TextType Type { get; set; }
        public string Value { get; set; }
    }

    public class TextItemContainer : IContentItem
    {
        public List<TextItem> Items { get; set; }
    }
}
