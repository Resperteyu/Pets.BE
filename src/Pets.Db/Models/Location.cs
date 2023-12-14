
using NetTopologySuite.Geometries;

namespace Pets.Db.Models
{
    public class Location
    {
        public int Id { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public Point GeoLocation { get; set; }

        public int AddressId { get; set; }
        public virtual Address Address { get; set; } = null!;
    }
}
