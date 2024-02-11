using System;

namespace Pets.API.Requests.Search
{
    public class SearchLitterParams
    {
        public byte? TypeId { get; set; }
        public int? BreedId { get; set; }
        public Guid UserId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public SearchRadiusType? SearchRadiusType { get; set; }
        public int? SearchRadius { get; set; }
    }
}

