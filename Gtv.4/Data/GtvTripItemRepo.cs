using System.Collections.Generic;
using System.Linq;
using gtv.Models;
using NoDb;

namespace gtv.Data
{

    public class GtvTripItemRepo
    {
        private const string TripItemsRoot = "tripitems/";

        private IBasicCommands<TripItem> _commands;
        private IBasicQueries<TripItem> _queries;

        public GtvTripItemRepo(IBasicCommands<TripItem> commands, IBasicQueries<TripItem> queries)
        {
            _commands = commands;
            _queries = queries;
        }

        public void Create(string triptag, TripItem tripitem)
        {
            _commands.CreateAsync($"{TripItemsRoot}{triptag}", tripitem.Tag, tripitem).Wait();
        }

        public void Update(string triptag, TripItem tripitem)
        {
            _commands.UpdateAsync($"{TripItemsRoot}{triptag}", tripitem.Tag, tripitem).Wait();
        }

        public void Delete(string triptag, string tripitemtag)
        {
            _commands.DeleteAsync($"{TripItemsRoot}{triptag}", tripitemtag).Wait();
        }

        public IEnumerable<TripItem> GetTripItems()
        {
            return _queries.GetAllAsync($"tripitems").Result;
        }

        public TripItem GetTripItem(string triptag, string tripitemtag)
        {
            return GetTripItems().Where(ti => ti.Tag == tripitemtag).FirstOrDefault();
        }

    }
}
