using gtv.Data;
using gtv.Models;
using gtv.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NoDb;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace gtv.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBasicQueries<Models.Trip> _tQ;
        private readonly IBasicCommands<Models.Trip> _tCom;
        private readonly IBasicQueries<Models.TripItem> _tiQ;
        private readonly IBasicCommands<Models.TripItem> _tiCom;

        public HomeController(
            IBasicCommands<Models.TripItem> tiCom, IBasicQueries<Models.TripItem> tiQ,
            IBasicCommands<Models.Trip> tCom, IBasicQueries<Models.Trip> tQ)
        {
            _tiCom = tiCom;
            _tiQ = tiQ;
            _tQ = tQ;
            _tCom = tCom;
        }

        private GtvTripRepo _tripRepo;
        public GtvTripRepo TripRepo
        {
            get { return _tripRepo == null ? _tripRepo = new GtvTripRepo(_tCom, _tQ) : _tripRepo; }
        }

        private GtvTripItemRepo _tripitemRepo;
        public GtvTripItemRepo TripItemRepo
        {
            get { return _tripitemRepo == null ? _tripitemRepo = new GtvTripItemRepo(_tiCom, _tiQ) : _tripitemRepo; }
        }

        public IActionResult Index()
        {
            var trips = TripRepo.GetAllTrips().OrderBy(t => t.TripOrder);
            var tripItems = TripItemRepo.GetTripItems().OrderByDescending(t => t.PublishDate);
            return View(new Home
            {
                Carousels = trips,
                TripItems = tripItems.Select(ti => new HomeTripItem
                {
                    Tag = ti.Tag,
                    TripTag = ti.TripTag,
                    HeaderImage = ti.HeaderImage,
                    PublishDate = ti.PublishDate,
                    SubTitle = ti.SubTitle,
                    Title = ti.Title,
                    TripTitle = trips
                        .Where(t => t.TripTag == ti.TripTag)
                        .OrderBy(t => t.TripOrder)
                        .Select(t => t.Title)
                        .FirstOrDefault()
                })
            });
        }

        [HttpGet]
        [Route("Trips/{tag}/")]
        public IActionResult Trip(string tag)
        {
            var trip = TripRepo.GetAllTrips().Where(t => t.TripTag == tag).FirstOrDefault();
            var tripItems = TripItemRepo.GetTripItems().Where(ti => ti.TripTag == trip.TripTag).OrderBy(t => t.PublishDate);
            return View("Trip", new ViewModels.Trip
            {
                TripTag = trip.TripTag,
                Title = trip.Title,
                Image = trip.Image,
                TripHeader = trip.TripHeader,
                TripDescription = GetTripItemContent(trip.TripDescription),
                TripItems = tripItems
            });
        }

        [HttpGet]
        [Route("Trips/{tag}/{id}")]
        public IActionResult TripItem(string tag, string id)
        {
            var trip = TripItemRepo.GetTripItem(tag, id);
            return View("TripItem", new ViewModels.TripItem
            {
                TripTag = trip.TripTag,
                TripItemTag = trip.Tag,
                Author = trip.Author.ToLower(),
                LessonTag = trip.LessonTag,
                Title = trip.Title,
                HeaderImage = trip.HeaderImage,
                TripContent = GetTripItemContent(TripItemRepo.GetTripItem(tag, id).TripContent),
                PublishDate = trip.PublishDate,
                SubTitle = trip.SubTitle
            });
        }

        [HttpGet]
        [Route("Trips")]
        public IActionResult Trips()
        {
            var vm = new List<ViewModels.Trip>();
            foreach (var trip in TripRepo.GetAllTrips().OrderBy(t => t.TripOrder))
            {
                vm.Add(new ViewModels.Trip
                {
                    TripTag = trip.TripTag,
                    Title = trip.Title,
                    Image = trip.Image,
                    TripHeader = trip.TripHeader,
                    TripDescription = GetTripItemContent(trip.TripDescription)
                });
            }
            return View("Trips", vm);
        }

        [HttpGet]
        [Route("About")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        [Route("GtvPics")]
        public IActionResult GtvPics()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static List<IContentItem> GetTripItemContent(string tripContent)
        {
            var contentParts = Regex.Split(tripContent, @"(?=[#&*_-]|[\r\n])|(?<=[#&*_-]|[\r\n])");
            var content = new List<IContentItem>();
            var textblock = new TextItemContainer();
            textblock.Items = new List<TextItem>();
            var listblock = new ListItem();
            listblock.Items = new List<string>();

            var state = CurrentItem.notset;
            for (var i = 0; i <= contentParts.GetUpperBound(0); i++)
            {
                var isBreak = (contentParts[i] == "\r" && contentParts[i + 1] == "\n");
                var isListItem = (contentParts[i] == "-" && contentParts[i + 1] == "-" && contentParts[i + 3] == "\r" && contentParts[i + 4] == "\n");
                var isBold = (contentParts[i] == "*" && contentParts[i + 1] == "*" && contentParts[i + 3] == "*" && contentParts[i + 4] == "*");
                var isItalic = (contentParts[i] == "_" && contentParts[i + 1] == "_" && contentParts[i + 3] == "_" && contentParts[i + 4] == "_");
                var isLargeImage = (contentParts[i] == "#" && contentParts[i + 1] == "#" && contentParts[i + 3] == "#" && contentParts[i + 4] == "#");
                var isMediumImage = (contentParts[i] == "&" && contentParts[i + 1] == "&" && contentParts[i + 3] == "&" && contentParts[i + 4] == "&");

                if ((state == CurrentItem.list || state == CurrentItem.notset) && isListItem)
                {
                    state = CurrentItem.list;
                    listblock.Items.Add(contentParts[i + 2]);
                    i = i + 4;
                    continue;
                }
                if (state == CurrentItem.list && !isListItem)
                {
                    content.Add(listblock);
                    listblock = new ListItem();
                    listblock.Items = new List<string>();
                    state = CurrentItem.notset;
                }
                if (state == CurrentItem.text || state == CurrentItem.notset && isBreak)
                {
                    if (textblock.Items.Any())
                    {
                        content.Add(textblock);
                        textblock = new TextItemContainer();
                        textblock.Items = new List<TextItem>();
                    }
                    state = CurrentItem.notset;
                    i = i + 1;
                    continue;
                }
                if (state == CurrentItem.text || state == CurrentItem.notset && isBold)
                {
                    textblock.Items.Add(new TextItem { Type = TextType.bold, Value = contentParts[i + 2] });
                    i = i + 5;
                    continue;
                }
                if (state == CurrentItem.text || state == CurrentItem.notset && isItalic)
                {
                    textblock.Items.Add(new TextItem { Type = TextType.italic, Value = contentParts[i + 2] });
                    i = i + 5;
                    continue;
                }
                if (state == CurrentItem.text && (isLargeImage || isMediumImage))
                {
                    content.Add(textblock);
                    textblock = new TextItemContainer();
                    textblock.Items = new List<TextItem>();
                    state = CurrentItem.notset;
                }
                if (isLargeImage)
                {
                    content.Add(new LargeImageItem
                    {
                        Image = contentParts[i + 2].Replace("[", "").Replace("]", "").Split('|')[0],
                        Caption = contentParts[i + 2].Replace("[", "").Replace("]", "").Split('|')[1]
                    });
                    i = i + 5;
                    continue;
                }
                if (isMediumImage)
                {
                    content.Add(new MediumImageItem
                    {
                        Image = contentParts[i + 2].Replace("[", "").Replace("]", "").Split('|')[0],
                        Caption = contentParts[i + 2].Replace("[", "").Replace("]", "").Split('|')[1]
                    });
                    i = i + 5;
                    continue;
                }
                if (textblock.Items.Any() && textblock.Items.Last().Type == TextType.text)
                {
                    textblock.Items.Last().Value += contentParts[i];
                }
                else
                {
                    textblock.Items.Add(new TextItem { Type = TextType.text, Value = contentParts[i] });
                }
            }
            content.Add(textblock);
            return content;
        }
    }
}
