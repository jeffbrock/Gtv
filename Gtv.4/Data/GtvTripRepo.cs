using System.Collections.Generic;
using gtv.Models;
using NoDb;
using System.Linq;

namespace gtv.Data
{
    public class GtvTripRepo 
    {
        private const string TripsRoot = "trips";

        private IBasicQueries<Trip> _queries;
        private IBasicCommands<Trip> _commands;

        public GtvTripRepo(IBasicCommands<Trip> commands, IBasicQueries<Trip> queries)
        {
            _queries = queries;
            _commands = commands;
        }

        public void Create(string triptag, Trip trip)
        {
            _commands.CreateAsync($"{TripsRoot}", triptag, trip).Wait();
        }

        public void Update(string triptag, Trip trip)
        {
            _commands.UpdateAsync($"{TripsRoot}", triptag, trip).Wait();
        }

        public void Delete(string triptag)
        {
            _commands.DeleteAsync($"{TripsRoot}", triptag).Wait();
        }

        public IEnumerable<Trip> GetAllTrips()
        {
            return _queries.GetAllAsync($"{TripsRoot}").Result;
        }

        public Trip GetTrip(string triptag)
        {
            return GetAllTrips().Where(t => t.TripTag == triptag).FirstOrDefault();
        }
    }
}
