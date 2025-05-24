using NetTopologySuite.Geometries;
using System.Text.Json.Serialization;

namespace Pets.API.Responses.Dtos
{
    public class LocationDto
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Point GeoLocation { get; set; }
    }
}
