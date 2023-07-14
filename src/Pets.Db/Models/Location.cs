
using NetTopologySuite.Geometries;

namespace Pets.Db.Models
{
    public class Location
    {
        public int Id { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public Point GeoLocation { get; set; }

        public ICollection<ApplicationUser> ApplicationUsers { get; } = new List<ApplicationUser>();
    }
}
